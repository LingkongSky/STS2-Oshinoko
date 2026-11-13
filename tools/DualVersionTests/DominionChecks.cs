using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

internal static class DominionChecks
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly List<(Type Power, Creature Target, decimal Amount)> Applications = [];
    private static readonly List<(Type Power, int? Amount)> PowerTips = [];
    private static readonly List<PowerModel> RegenerationInstances = [];
    private static CombatState _multiplayerCombat = null!;
    private static bool _failApplication;
    private static int _assertions;

    internal static async Task Run(Assembly mod, Harmony harmony)
    {
        var game = typeof(AbstractModel).Assembly;
        var getDominion = typeof(EncounterModel).GetMethod("GetDominion");
        var aiType = mod.GetType("Oshinoko.Scripts.Encounters.AiEncounter", true)!;
        var hikaruType = mod.GetType("Oshinoko.Scripts.Encounters.KamikiHikaruEncounter", true)!;
        var dreadType = game.GetType("MegaCrit.Sts2.Core.Models.Dread.DreadRunModel");
        var models = game.GetTypes().Where(t => !t.IsAbstract && !t.ContainsGenericParameters
            && (typeof(PowerModel).IsAssignableFrom(t)
                || t.BaseType?.FullName == "MegaCrit.Sts2.Core.Models.Dread.DominionModel"))
            .Concat(new[] { aiType, hikaruType, typeof(KaiserCrabBoss), typeof(Hive), typeof(Glory) });
        if (dreadType != null) models = models.Append(dreadType);
        ModelDb.ResetForTest();
        ModelDb.Init(models.ToArray());
        try
        {
            if (getDominion == null)
            {
                mod.GetType("Oshinoko.Scripts.Compatibility.BossDominionCompat", true)!
                    .GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, null);
                Check(dreadType == null, "Old host has no Dread mode");
                Check(!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "Oshinoko.Dread"),
                    "Old host never creates a Dread assembly");
                Check(!ModelDb.All.Any(m => m.GetType().Name.EndsWith("Dominion", StringComparison.Ordinal)),
                    "Old host does not register unsupported dominions");
                Console.WriteLine($"DOMINION: {_assertions} assertions; old host skips Dread types and patches");
                return;
            }

            Check(Harmony.GetPatchInfo(getDominion)!.Owners.Contains(harmony.Id), "Encounter bridge installed through PatchAll");
            CheckAssemblyAssociation(mod, harmony);
            _multiplayerCombat = Raw<CombatState>();
            typeof(CombatState).GetField("_allies", Instance)!.SetValue(_multiplayerCombat,
                Enumerable.Range(0, 4).Select(_ => new Creature(Raw<Player>(), 50, 50)).ToList());
            typeof(CombatState).GetField("_enemies", Instance)!.SetValue(_multiplayerCombat, new List<Creature>());
            typeof(CombatState).GetField("<RunState>k__BackingField", Instance)!.SetValue(_multiplayerCombat, Raw<RunState>());
            var records = new List<(MethodBase Original, MethodInfo Prefix)>();
            void Patch(MethodBase original, string prefix)
            {
                var method = typeof(DominionChecks).GetMethod(prefix, BindingFlags.Static | BindingFlags.Public)!;
                harmony.Patch(original, prefix: new HarmonyMethod(method));
                records.Add((original, method));
            }
            // Execute the real generic/group command wrappers, capturing only the
            // final application. Godot visuals and combat scheduling are excluded.
            Patch(typeof(CombatManager).GetProperty(nameof(CombatManager.IsEnding))!.GetMethod!, nameof(NotEnding));
            Patch(typeof(Creature).GetProperty(nameof(Creature.CanReceivePowers))!.GetMethod!, nameof(CanReceive));
            Patch(typeof(PowerCmd).GetMethod(nameof(PowerCmd.FindExistingInstanceForStacking))!, nameof(NoExistingPower));
            Patch(typeof(PowerCmd).GetMethods().Single(m => m.Name == nameof(PowerCmd.Apply) && !m.IsGenericMethod), nameof(CapturePower));
            Patch(typeof(HoverTipFactory).GetMethod(nameof(HoverTipFactory.FromPower), [typeof(PowerModel), typeof(int?)])!, nameof(CapturePowerTip));
            // Native logging initializes Godot.OS. Strip only log calls so the
            // real Dread lifecycle can run in this managed-only test host.
            var logging = typeof(DominionChecks).GetMethod(nameof(SkipNativeLogging))!;
            foreach (var methodName in new[] { "AfterRunLoaded", "ApplyCurrentDominion", "RemoveDominionIfRoomIsBoss" })
            {
                var original = dreadType!.GetMethod(methodName, Instance)!;
                harmony.Patch(original, transpiler: new HarmonyMethod(logging));
                records.Add((original, logging));
            }
            try
            {
                var vanilla = ModelDb.Encounter<KaiserCrabBoss>();
                Check(getDominion.Invoke(vanilla, null)!.GetType().Name == "KaiserCrabDominion", "Vanilla encounter retains its own dominion");
                foreach (var (encounterType, act, entry, expected) in new[]
                {
                    (aiType, (ActModel)ModelDb.Act<Hive>().MutableClone(), "HOSHINO_AI_DOMINION", new[] { (typeof(RegenPower), 5m) }),
                    (hikaruType, (ActModel)ModelDb.Act<Glory>().MutableClone(), "KAMIKI_HIKARU_DOMINION", new[] { (typeof(DebilitatePower), 1m), (typeof(VulnerablePower), 1m) }),
                })
                {
                    var encounter = ModelDb.GetByIdOrNull<EncounterModel>(ModelDb.GetId(encounterType))!;
                    var canonical = (AbstractModel)getDominion.Invoke(encounter, null)!;
                    Check(canonical.Id.Entry == entry, "Distinct native dominion ID");
                    var description = (LocString)canonical.GetType().GetProperty("Description")!.GetValue(canonical)!;
                    Check(description.LocTable == "ascension" && description.LocEntryKey == $"DREAD.{entry[..^"_DOMINION".Length]}.description",
                        "Native description resolves to the supplied localization key");
                    Check(canonical.IsCanonical && ModelDb.All.Contains(canonical), "Dominion registered during ModelDb.Init, before network ID initialization");
                    Check(ReferenceEquals(canonical, getDominion.Invoke(encounter, null)), "Encounter returns the canonical registered model");
                    PowerTips.Clear();
                    var tips = (IEnumerable<IHoverTip>)canonical.GetType().GetProperty("ExtraHoverTips")!.GetValue(canonical)!;
                    Check(tips.Count() >= expected.Length, "Power hover tips are available");
                    foreach (var (power, amount) in expected)
                        Check(PowerTips.Contains((power, (int)amount)), "Power hover tips use the specified amounts");
                    var active = canonical.MutableClone();
                    Check(active.ShouldReceiveCombatHooks, "Native combat hooks enabled");
                    foreach (var (alliesCount, enemiesCount) in new[] { (1, 1), (4, 3), (0, 0) })
                    {
                        var allies = Enumerable.Range(0, alliesCount).Select(_ => Raw<Creature>()).ToList();
                        var enemies = Enumerable.Range(0, enemiesCount).Select(_ => Raw<Creature>()).ToList();
                        var combat = Raw<CombatState>();
                        typeof(CombatState).GetField("_allies", Instance)!.SetValue(combat, allies);
                        typeof(CombatState).GetField("_enemies", Instance)!.SetValue(combat, enemies);
                        var room = new CombatRoom(combat);
                        Applications.Clear();
                        RegenerationInstances.Clear();
                        await active.AfterRoomEntered(room);
                        Check(Applications.Count == (alliesCount + enemiesCount) * expected.Length, "Exact opening application count");
                        foreach (var creature in allies.Concat(enemies))
                        foreach (var (power, amount) in expected)
                            Check(Applications.Count(a => a.Power == power && ReferenceEquals(a.Target, creature) && a.Amount == amount) == 1,
                                "Every ally and enemy receives each specified amount exactly once");
                        foreach (var power in RegenerationInstances)
                            Check(power.GetScaledAmountForMultiplayer(_multiplayerCombat, null, 5, allies.Concat(enemies).First(), null) == 22,
                                "Normal Regen scaling restored immediately after the opening application");
                        var count = Applications.Count;
                        await active.BeforeCombatStart();
                        Check(Applications.Count == count, "BeforeCombatStart does not repeat opening effects");
                        await active.AfterCreatureAddedToCombat(Raw<Creature>());
                        Check(Applications.Count == count, "Later spawns do not repeat opening-only effects");
                    }
                    Applications.Clear();
                    await active.AfterRoomEntered(null!);
                    Check(Applications.Count == 0, "Non-combat rooms have no opening powers");
                    if (entry == "HOSHINO_AI_DOMINION")
                    {
                        RegenerationInstances.Clear();
                        _failApplication = true;
                        try
                        {
                            await active.AfterRoomEntered(new CombatRoom(_multiplayerCombat));
                            throw new Exception("Opening failure was swallowed");
                        }
                        catch (InvalidOperationException ex) when (ex.Message == "opening sentinel") { }
                        finally { _failApplication = false; }
                        Check(RegenerationInstances.Single().GetScaledAmountForMultiplayer(_multiplayerCombat, null, 5,
                            _multiplayerCombat.Creatures[0], null) == 22, "Regen scaling restored after a failed application");
                    }

                    // Use the actual Dread run model to select/restore the act's
                    // dominion, then remove it upon entering the boss encounter.
                    act.SetBossEncounter(encounter);
                    var run = Raw<RunState>();
                    typeof(RunState).GetProperty(nameof(RunState.Acts))!.SetValue(run, new[] { act });
                    var dread = ModelDb.GetByIdOrNull<AbstractModel>(ModelDb.GetId(dreadType!))!.MutableClone();
                    dreadType!.GetMethod("OnRunCreated")!.Invoke(dread, [run]);
                    dreadType.GetMethod("AfterRunLoaded")!.Invoke(dread, null);
                    var dominionProperty = typeof(RunState).GetProperty("Dominion")!;
                    var restored = (AbstractModel)dominionProperty.GetValue(run)!;
                    Check(restored.GetType() == canonical.GetType() && restored.IsMutable, "Native Dread load restores a mutable custom dominion");
                    var bossRoom = new CombatRoom((EncounterModel)encounter.MutableClone(), run);
                    await dread.BeforeRoomEntered(bossRoom);
                    Check(dominionProperty.GetValue(run) == null, "Native Dread removes custom dominion in boss fight");
                    Check(typeof(CombatState).GetProperty("Dominion")!.GetValue(bossRoom.CombatState) == null, "Boss combat also clears its dominion");
                }
            }
            finally
            {
                foreach (var (original, prefix) in records) harmony.Unpatch(original, prefix);
            }
            Console.WriteLine($"DOMINION: {_assertions} assertions; native encounter/load/boss removal/network sorting; opening targets and amounts; fixed multiplayer Regen and cleanup after success/failure");
        }
        finally
        {
            ModelDb.ResetForTest();
        }
    }

    private static T Raw<T>() => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

    private static void CheckAssemblyAssociation(Assembly mod, Harmony harmony)
    {
        var associate = typeof(ModManager).GetMethod("AssociateAssemblyWithMod")!;
        var logging = typeof(DominionChecks).GetMethod(nameof(SkipNativeLogging))!;
        harmony.Patch(associate, transpiler: new HarmonyMethod(logging));
        var mods = (List<Mod>)typeof(ModManager).GetField("_mods", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        var owner = new Mod { path = "managed-test-fixture", state = ModLoadState.Loaded, manifest = new ModManifest { id = "Oshinoko" } };
        mods.Add(owner);
        try
        {
            mod.GetType("Oshinoko.Scripts.Compatibility.BossDominionCompat", true)!
                .GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, null);
            var types = ModelDb.All.Select(m => m.GetType()).Where(t => t.Assembly.GetName().Name == "Oshinoko.Dread").ToArray();
            Check(types.Length == 2 && owner.assemblies.Contains(types[0].Assembly), "Dynamic dominions associated with their mod before assembly-map initialization");
            AssemblyInfo.Init();
            var sorted = ContentSorter<ModelId>.Sort(types, ModelDb.GetId);
            Check(sorted.Count == 2 && sorted.All(item => ReferenceEquals(item.mod, owner)), "Native network ID sorter recognizes both custom dominions as Oshinoko content");
            var reverse = ContentSorter<ModelId>.Sort(types.Reverse(), ModelDb.GetId);
            Check(sorted.Select(item => item.id).SequenceEqual(reverse.Select(item => item.id)), "Network model ordering is deterministic");
        }
        finally
        {
            AssemblyInfo.ClearForTests();
            mods.Remove(owner);
            harmony.Unpatch(associate, logging);
        }
    }

    private static void Check(bool condition, string message)
    {
        _assertions++;
        if (!condition) throw new InvalidOperationException(message);
    }

    public static bool NotEnding(ref bool __result) { __result = false; return false; }
    public static bool CanReceive(ref bool __result) { __result = true; return false; }
    public static bool NoExistingPower(ref PowerModel? __result) { __result = null; return false; }

    public static bool CapturePowerTip(PowerModel __0, int? __1, ref IHoverTip __result)
    {
        PowerTips.Add((__0.GetType(), __1));
        __result = Raw<HoverTip>();
        return false;
    }

    public static IEnumerable<CodeInstruction> SkipNativeLogging(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo method
                && method.DeclaringType?.FullName == "MegaCrit.Sts2.Core.Logging.Log")
            {
                for (var i = 0; i < method.GetParameters().Length; i++)
                    yield return new CodeInstruction(OpCodes.Pop);
            }
            else yield return instruction;
        }
    }

    public static bool CapturePower(object[] __args, ref Task __result)
    {
        Check(__args[0] is ThrowingPlayerChoiceContext, "Dominion uses the native no-choice context");
        Check(__args[4] == null && __args[5] == null, "Opening effects have no creature or card source");
        var power = (PowerModel)__args[1]!;
        var target = (Creature)__args[2]!;
        var amount = (decimal)__args[3]!;
        if (power is RegenPower)
        {
            Check(power.GetScaledAmountForMultiplayer(_multiplayerCombat, null, amount, target, null) == 5,
                "Opening Regen stays at five through the actual native multiplayer scaling method");
            RegenerationInstances.Add(power);
        }
        Applications.Add((power.GetType(), target, amount));
        __result = _failApplication ? Task.FromException(new InvalidOperationException("opening sentinel")) : Task.CompletedTask;
        return false;
    }
}
