using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

internal static class ResourceUsageChecks
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static IEnumerable<AbstractModel> _listeners = [];
    private static readonly List<(Creature Target, decimal Amount, ValueProp Props, CardPlay? Play)> Blocks = [];
    private static int _assertions;

    internal static async Task Run(Assembly mod, Harmony harmony)
    {
        var powerTypes = new[] { "ShinePower", "TurnShinePower", "TempShinePower", "RevengePower", "TurnRevengePower", "TempRevengePower" }
            .Select(name => mod.GetType($"Oshinoko.Scripts.Powers.{name}", true)!).ToArray();
        var tracker = mod.GetType("Oshinoko.Scripts.Cards.Other.ResourceUsageTracker", true)!;
        var cardType = mod.GetType("Oshinoko.Scripts.Cards.Ruby.DailyAccumulation", true)!;
        var onPlay = cardType.GetMethod("OnPlay", Instance)!;
        var listenersMethod = typeof(Hook).GetMethod("IterateCombatHookListeners", BindingFlags.Static | BindingFlags.NonPublic)!;
        var blockMethod = typeof(CreatureCmd).GetMethod(nameof(CreatureCmd.GainBlock),
            [typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(CardPlay), typeof(bool)])!;
        var listenersPrefix = typeof(ResourceUsageChecks).GetMethod(nameof(CaptureListeners))!;
        var blockPrefix = typeof(ResourceUsageChecks).GetMethod(nameof(CaptureBlock))!;
        harmony.Patch(listenersMethod, prefix: new HarmonyMethod(listenersPrefix));
        harmony.Patch(blockMethod, prefix: new HarmonyMethod(blockPrefix));
        try
        {
            // Enumerate every subset of the six resources. Only listener discovery
            // and the final block command are intercepted; dispatch and OnPlay are real.
            for (var mask = 0; mask < 64; mask++)
            {
                var combat = Raw<CombatState>();
                combat.RoundNumber = 1;
                var player = CreatePlayer(combat);
                var ally = CreatePlayer(combat);
                var enemy = Raw<Creature>();
                enemy.CombatState = combat;
                var ownPowers = powerTypes.Select(type => CreatePower(type, player.Creature)).ToArray();
                var allyPowers = powerTypes.Select(type => CreatePower(type, ally.Creature)).ToArray();
                var selected = Enumerable.Range(0, 6).Where(i => (mask & (1 << i)) != 0).ToArray();
                _listeners = selected.Select(i => ownPowers[i]).Concat(allyPowers).ToArray();
                var context = new BlockingPlayerChoiceContext();
                var play = Raw<CardPlay>();
                var card = (CardModel)RuntimeHelpers.GetUninitializedObject(cardType);
                typeof(AbstractModel).GetField("<IsMutable>k__BackingField", Instance)!.SetValue(card, true);
                card.Owner = player;
                var gained = new int[2];
                var spent = new int[2];
                var allyGained = new int[2];

                await CheckBlock(onPlay, card, context, play, 0, $"mask {mask}: initially empty");
                foreach (var i in selected)
                {
                    var amount = i + 1;
                    await Hook.AfterPowerAmountChanged(combat, context, ownPowers[i], amount, player.Creature, null);
                    gained[i / 3] += amount;
                    CheckTotals(tracker, player, gained, spent, $"mask {mask}: gain {powerTypes[i].Name}");
                    CheckTotals(tracker, ally, allyGained, [0, 0], $"mask {mask}: own gain does not affect ally");
                    await CheckBlock(onPlay, card, context, play, gained.Sum(), $"mask {mask}: after gain");
                }

                // The same resource types on another player must only count for that player.
                for (var i = 0; i < 6; i++)
                {
                    await Hook.AfterPowerAmountChanged(combat, context, allyPowers[i], 10 + i, ally.Creature, null);
                    allyGained[i / 3] += 10 + i;
                    CheckTotals(tracker, player, gained, spent, $"mask {mask}: ally resource ignored");
                    CheckTotals(tracker, ally, allyGained, [0, 0], $"mask {mask}: ally resource counted once");
                }
                await CheckBlock(onPlay, card, context, play, gained.Sum(), $"mask {mask}: after ally changes");

                foreach (var owner in new[] { player.Creature, ally.Creature, enemy })
                foreach (var type in new[] { typeof(StrengthPower), typeof(DexterityPower), typeof(WeakPower) })
                foreach (var amount in new[] { 4m, -2m, 0m })
                {
                    var unrelated = CreatePower(type, owner);
                    await Hook.AfterPowerAmountChanged(combat, context, unrelated, amount, owner, null);
                    CheckTotals(tracker, player, gained, spent, $"mask {mask}: unrelated {type.Name} {amount}");
                    CheckTotals(tracker, ally, allyGained, [0, 0], $"mask {mask}: unrelated change ignored by ally");
                }
                await CheckBlock(onPlay, card, context, play, gained.Sum(), $"mask {mask}: after unrelated changes");

                foreach (var i in selected)
                {
                    await Hook.AfterPowerAmountChanged(combat, context, ownPowers[i], -1, player.Creature, null);
                    await Hook.AfterPowerAmountChanged(combat, context, ownPowers[i], 0, player.Creature, null);
                    spent[i / 3]++;
                    CheckTotals(tracker, player, gained, spent, $"mask {mask}: loss preserves lifetime gains");
                    await CheckBlock(onPlay, card, context, play, gained.Sum(), $"mask {mask}: after resource loss");
                }

                if (selected.Length > 0)
                {
                    var i = selected[0];
                    combat.RoundNumber++;
                    combat.CurrentSide = CombatSide.Enemy;
                    await Hook.AfterPowerAmountChanged(combat, context, ownPowers[i], 2, player.Creature, null);
                    gained[i / 3] += 2;
                    CheckTotals(tracker, player, gained, spent, $"mask {mask}: new round preserves gains");
                    await CheckBlock(onPlay, card, context, play, gained.Sum(), $"mask {mask}: next round");
                }

                player.Creature.CombatState = Raw<CombatState>();
                CheckTotals(tracker, player, [0, 0], [0, 0], $"mask {mask}: new combat resets totals");
                await CheckBlock(onPlay, card, context, play, 0, $"mask {mask}: new combat grants no old block");
            }
            Console.WriteLine($"RESOURCE_USAGE: {_assertions} assertions; all 64 Shine/Revenge subsets; owner isolation; unrelated buffs/debuffs; losses; round/combat resets; actual DailyAccumulation OnPlay block arguments");
        }
        finally
        {
            harmony.Unpatch(listenersMethod, listenersPrefix);
            harmony.Unpatch(blockMethod, blockPrefix);
            _listeners = [];
            Blocks.Clear();
        }
    }

    private static Player CreatePlayer(CombatState combat)
    {
        var player = Raw<Player>();
        var creature = new Creature(player, 50, 50) { CombatState = combat };
        typeof(Player).GetField("<Creature>k__BackingField", Instance)!.SetValue(player, creature);
        return player;
    }

    private static PowerModel CreatePower(Type type, Creature owner)
    {
        var power = (PowerModel)RuntimeHelpers.GetUninitializedObject(type);
        typeof(AbstractModel).GetField("<IsMutable>k__BackingField", Instance)!.SetValue(power, true);
        typeof(PowerModel).GetField("_owner", Instance)!.SetValue(power, owner);
        return power;
    }

    private static void CheckTotals(Type tracker, Player player, int[] gained, int[] spent, string label)
    {
        var names = new[] { "GetTotalShineGained", "GetTotalRevengeGained", "GetTotalShineSpent", "GetTotalRevengeSpent" };
        var expected = new[] { gained[0], gained[1], spent[0], spent[1] };
        for (var i = 0; i < names.Length; i++)
            Equal(expected[i], (int)tracker.GetMethod(names[i])!.Invoke(null, [player])!, $"{label}: {names[i]}");
    }

    private static async Task CheckBlock(MethodInfo onPlay, CardModel card, PlayerChoiceContext context, CardPlay play, int expected, string label)
    {
        var count = Blocks.Count;
        await (Task)onPlay.Invoke(card, [context, play])!;
        Equal(expected > 0 ? 1 : 0, Blocks.Count - count, $"{label}: block command count");
        if (expected > 0)
            Equal((card.Owner.Creature, (decimal)expected, ValueProp.Move, (CardPlay?)play), Blocks[^1], $"{label}: block arguments");
    }

    private static T Raw<T>() => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

    private static void Equal<T>(T expected, T actual, string label)
    {
        _assertions++;
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{label}: expected {expected}, got {actual}");
    }

    public static bool CaptureListeners(ref IEnumerable<AbstractModel> __result)
    {
        __result = _listeners;
        return false;
    }

    public static bool CaptureBlock(Creature __0, decimal __1, ValueProp __2, CardPlay? __3, ref Task<decimal> __result)
    {
        Blocks.Add((__0, __1, __2, __3));
        __result = Task.FromResult(__1);
        return false;
    }
}
