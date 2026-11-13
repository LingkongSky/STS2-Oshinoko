using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

public static class RuntimeChecks
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static int _assertions;
    private static object?[]? _damageArguments;
    private static Task<IEnumerable<DamageResult>> _damageResult = Task.FromResult(Enumerable.Empty<DamageResult>());
    private static readonly List<(MonsterModel Monster, MoveState Move, bool Force)> Moves = [];
    private static IEnumerable<AbstractModel> _hookListeners = [];

    public static async Task Run(Assembly mod)
    {
        Console.WriteLine($"RITSU_RUNTIME={mod.GetType("Oshinoko.Scripts.Monsters.KamikiHikaru", true)!.BaseType!.Assembly.GetName()}");
        var harmony = new Harmony("Oshinoko.DualVersionTests");
        try
        {
            // This is the same PatchAll registration performed by Entry.Init.
            harmony.PatchAll(mod);
            CheckPowerRegistration(mod);
            await DominionChecks.Run(mod, harmony);
            BossEncounterChecks.Run(mod);
            await CheckDeathHook(mod, harmony);
            await CheckDamageBridge(mod, harmony);
            Console.WriteLine($"RUNTIME: {_assertions} assertions; real Harmony patches; game commands captured without Godot or saves");
        }
        finally
        {
            harmony.UnpatchAll(harmony.Id);
        }
    }

    private static void CheckPowerRegistration(Assembly mod)
    {
        var game = typeof(PowerModel).Assembly;
        var powers = game.GetTypes().Concat(mod.GetTypes()).Where(type =>
            type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters && typeof(PowerModel).IsAssignableFrom(type)).ToArray();
        ModelDb.ResetForTest();
        try
        {
            // Use real constructors and the game's real registration loop.
            // Merely enumerating types or using uninitialized objects misses ID collisions.
            ModelDb.Init(powers);
            foreach (var type in powers)
            {
                var id = ModelDb.GetId(type);
                Equal(type, ModelDb.GetByIdOrNull<PowerModel>(id)?.GetType(), $"Registered power identity: {id}");
            }
            var encore = mod.GetType("Oshinoko.Scripts.Powers.OshinokoEncorePower", true)!;
            var encoreId = ModelDb.GetId(encore);
            Equal("POWER.OSHINOKO_ENCORE_POWER", encoreId.ToString(), "Mod Encore has its own ID");
            Equal<Type?>(null, mod.GetType("Oshinoko.Scripts.Powers.EncorePower"), "Old ambiguous mod type removed");
            var vanilla = game.GetType("MegaCrit.Sts2.Core.Models.Powers.EncorePower");
            var originalId = new ModelId("POWER", "ENCORE_POWER");
            Equal(vanilla, ModelDb.GetByIdOrNull<PowerModel>(originalId)?.GetType(), "Original Encore ID remains available to the base game");
            Console.WriteLine($"MODEL_REGISTRATION: {powers.Length} power models constructed and registered; Encore IDs separated");
        }
        finally
        {
            ModelDb.ResetForTest();
        }
    }

    private static async Task CheckDeathHook(Assembly mod, Harmony harmony)
    {
        var bossType = mod.GetType("Oshinoko.Scripts.Monsters.KamikiHikaru", true)!;
        var hook = typeof(AbstractModel).GetMethod(nameof(AbstractModel.AfterDeath))!;
        Equal(true, Harmony.GetPatchInfo(hook)!.Owners.Contains(harmony.Id), "Death hook installed through PatchAll");
        harmony.Patch(typeof(MonsterModel).GetMethod(nameof(MonsterModel.SetMoveImmediate))!,
            prefix: new HarmonyMethod(typeof(RuntimeChecks), nameof(CaptureMove)));
        var owner = Raw<Creature>();
        var other = Raw<Creature>();
        var context = new BlockingPlayerChoiceContext();
        var source = (CardModel)RuntimeHelpers.GetUninitializedObject(mod.GetType("Oshinoko.Scripts.Cards.Aqua.AquaStrike", true)!);
        foreach (var phase in new[] { 1, 2 })
        foreach (var creature in new[] { owner, other })
        foreach (var prevented in new[] { false, true })
        foreach (var withMove in new[] { false, true })
        foreach (var withSource in new[] { false, true })
        {
            var boss = (MonsterModel)RuntimeHelpers.GetUninitializedObject(bossType);
            var phaseField = bossType.GetField("_phase", Instance)!;
            phaseField.SetValue(boss, Enum.ToObject(phaseField.FieldType, phase));
            typeof(MonsterModel).GetField("_creature", Instance)!.SetValue(boss, owner);
            var move = Raw<MoveState>();
            if (withMove) bossType.GetField("_deadState", Instance)!.SetValue(boss, move);
            var count = Moves.Count;
            await InvokeDeath(hook, boss, context, creature, prevented, withSource ? source : null);
            var transition = phase == 1 && ReferenceEquals(creature, owner) && !prevented;
            Equal(transition, (bool)bossType.GetField("_isReviving", Instance)!.GetValue(boss)!, "Original death guard and revival flag");
            Equal(transition && withMove ? 1 : 0, Moves.Count - count, "Transition selected exactly once");
            if (transition && withMove)
            {
                Equal((boss, move, true), Moves[^1], "Original phase transition state and force flag");
                await InvokeDeath(hook, boss, context, creature, prevented, null);
                Equal(count + 2, Moves.Count, "Existing repeated-death behavior is retained");
            }
            Equal(phase == 1 || transition, boss.ShouldStopCombatFromEnding(), "Combat-end gate retained");
            Equal(!(ReferenceEquals(creature, owner) && phase == 1), boss.ShouldCreatureBeRemovedFromCombatAfterDeath(creature), "Death removal rule retained");
            Equal(!(ReferenceEquals(creature, owner) && transition), boss.ShouldAllowHitting(creature), "Revival hit guard retained");
        }
        var unrelated = (AbstractModel)RuntimeHelpers.GetUninitializedObject(mod.GetType("Oshinoko.Scripts.Powers.EscapePower", true)!);
        var before = Moves.Count;
        await InvokeDeath(hook, unrelated, context, owner, false, source);
        Equal(before, Moves.Count, "Other models retain the original base hook");
        await CheckDeathDispatcher(mod, harmony, owner, source);
        Console.WriteLine($"DEATH: actual {hook.GetParameters().Length}-argument game hook registered and executed");
    }

    private static async Task CheckDeathDispatcher(Assembly mod, Harmony harmony, Creature creature, CardModel source)
    {
        var bossType = mod.GetType("Oshinoko.Scripts.Monsters.KamikiHikaru", true)!;
        var boss = (MonsterModel)RuntimeHelpers.GetUninitializedObject(bossType);
        var phase = bossType.GetField("_phase", Instance)!;
        phase.SetValue(boss, Enum.ToObject(phase.FieldType, 1));
        typeof(MonsterModel).GetField("_creature", Instance)!.SetValue(boss, creature);
        var move = Raw<MoveState>();
        bossType.GetField("_deadState", Instance)!.SetValue(boss, move);
        _hookListeners = [boss];
        var listenerMethod = typeof(RunState).GetMethod(nameof(RunState.IterateHookListeners))!;
        var listenerPrefix = typeof(RuntimeChecks).GetMethod(nameof(CaptureListeners))!;
        harmony.Patch(listenerMethod, prefix: new HarmonyMethod(listenerPrefix));
        var originalNetId = LocalContext.NetId;
        try
        {
            LocalContext.NetId = 1;
            var run = Raw<RunState>();
            var dispatcher = typeof(Hook).GetMethod(nameof(Hook.AfterDeath))!;
            var args = dispatcher.GetParameters().Select(p => p.Name switch
            {
                "runState" => (object?)run,
                "combatState" => null,
                "creature" => creature,
                "source" => source,
                "wasRemovalPrevented" => false,
                "deathAnimLength" => 0.75f,
                _ => throw new InvalidOperationException($"Unknown death dispatcher argument: {p.Name}")
            }).ToArray();
            var count = Moves.Count;
            await (Task)dispatcher.Invoke(null, args)!;
            Equal(count + 1, Moves.Count, "Real game dispatcher reaches the compatibility death hook");
            Equal((boss, move, true), Moves[^1], "Real dispatcher preserves transition selection");
        }
        finally
        {
            LocalContext.NetId = originalNetId;
            harmony.Unpatch(listenerMethod, listenerPrefix);
        }
    }

    private static Task InvokeDeath(MethodInfo hook, AbstractModel model, PlayerChoiceContext context,
        Creature creature, bool prevented, CardModel? source)
    {
        object?[] args = hook.GetParameters().Length == 4
            ? [context, creature, prevented, 0.75f]
            : [context, creature, source, prevented, 0.75f];
        return (Task)hook.Invoke(model, args)!;
    }

    private static async Task CheckDamageBridge(Assembly mod, Harmony harmony)
    {
        var bridge = mod.GetType("Oshinoko.Scripts.Compatibility.CreatureDamageCompat", true)!
            .GetMethod("Damage", BindingFlags.NonPublic | BindingFlags.Static)!;
        var target = typeof(CreatureCmd).GetMethods().Single(m => m.Name == nameof(CreatureCmd.Damage)
            && m.GetParameters() is var p && p.Length is 7 or 8
            && p[1].ParameterType == typeof(IEnumerable<Creature>) && p[2].ParameterType == typeof(decimal));
        var recorder = typeof(RuntimeChecks).GetMethod(nameof(CaptureDamage))!;
        harmony.Patch(target, prefix: new HarmonyMethod(recorder));
        var owner = Raw<Creature>();
        var card = (CardModel)RuntimeHelpers.GetUninitializedObject(mod.GetType("Oshinoko.Scripts.Cards.Aqua.AquaStrike", true)!);
        var play = Raw<CardPlay>();
        var contexts = new PlayerChoiceContext[] { new BlockingPlayerChoiceContext(), new BlockingPlayerChoiceContext() };
        var targets = new IEnumerable<Creature>?[] { null, [], new[] { owner }, DeferredTargets(owner) };
        _damageResult = Task.FromResult(Enumerable.Empty<DamageResult>());
        foreach (var context in contexts)
        foreach (var creatures in targets)
        foreach (var amount in new[] { 0m, 1m, 9m, 13m, 12.5m, -2m })
        foreach (var flags in Enumerable.Range(0, 16))
        foreach (var dealer in new Creature?[] { null, owner })
        foreach (var source in new CardModel?[] { null, card })
        foreach (var cardPlay in new CardPlay?[] { null, play })
        {
            var props = (ValueProp)(flags << 1);
            object?[] args = [context, creatures, amount, props, dealer, source, cardPlay];
            _damageArguments = null;
            var task = (Task<IEnumerable<DamageResult>>)bridge.Invoke(null, args)!;
            Equal(true, ReferenceEquals(_damageResult, task), "Original damage task returned unchanged");
            Equal(target.GetParameters().Length, _damageArguments!.Length, "Correct runtime overload");
            for (var i = 0; i < 7; i++) Equal(args[i], _damageArguments[i], $"Damage argument {i} preserved");
            if (_damageArguments.Length == 8)
                Equal(Convert.ToInt64(target.GetParameters()[7].DefaultValue), Convert.ToInt64(_damageArguments[7]), "New optional hook flags retain the game's default");
            await task;
        }
        var sentinel = new InvalidOperationException("sentinel damage failure");
        _damageResult = Task.FromException<IEnumerable<DamageResult>>(sentinel);
        var failed = (Task<IEnumerable<DamageResult>>)bridge.Invoke(null, [contexts[0], null, 9m, ValueProp.Move, null, null, null])!;
        try { await failed; throw new Exception("Fault was swallowed"); }
        catch (InvalidOperationException ex) { Equal(true, ReferenceEquals(sentinel, ex), "Async damage exception preserved"); }
        harmony.Unpatch(target, recorder);
        foreach (var empty in new IEnumerable<Creature>?[] { null, Array.Empty<Creature>() })
        {
            var task = (Task<IEnumerable<DamageResult>>)bridge.Invoke(null, [contexts[0], empty, 9m, ValueProp.Move, null, null, null])!;
            Equal(0, (await task).Count(), "Actual unpatched game damage accepts null/empty targets");
        }
        Console.WriteLine($"DAMAGE: actual {target.GetParameters().Length}-argument overload; parameters, defaults, task and exceptions verified");
    }

    private static IEnumerable<Creature> DeferredTargets(Creature creature)
    {
        yield return creature;
        throw new InvalidOperationException("The bridge must not enumerate targets");
    }

    private static T Raw<T>() => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

    private static void Equal<T>(T expected, T actual, string message)
    {
        _assertions++;
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{message}: expected {expected}, got {actual}");
    }

    public static bool CaptureMove(MonsterModel __instance, MoveState __0, bool __1)
    {
        Moves.Add((__instance, __0, __1));
        return false;
    }

    public static bool CaptureDamage(object[] __args, ref Task<IEnumerable<DamageResult>> __result)
    {
        _damageArguments = __args;
        __result = _damageResult;
        return false;
    }

    public static bool CaptureListeners(ref IEnumerable<AbstractModel> __result)
    {
        __result = _hookListeners;
        return false;
    }
}
