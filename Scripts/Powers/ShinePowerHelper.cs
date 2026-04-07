using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

// 数值持续类型：永久、回合、临时。
public enum ValueDuration
{
    Permanent,
    Turn,
    Temp,
}

// 用于记录“临时资源是由哪张牌赋予的”，从而在该牌自己的 AfterCardPlayed 中跳过一次，
// 避免出现“刚获得就立刻被消耗”的问题。
// 注意：这里只对 cardSource != null 的情况生效，因此非打牌时机获得的临时资源不会被额外跳过。
public static class TempPowerSourceTracker
{
    private static readonly Dictionary<Creature, HashSet<CardModel>> TempShineSources = new();

    private static readonly Dictionary<Creature, HashSet<CardModel>> TempRevengeSources = new();

    public static void RegisterTempShineSource(Creature target, CardModel? cardSource)
    {
        if (cardSource == null)
        {
            return;
        }

        if (!TempShineSources.TryGetValue(target, out var sources))
        {
            sources = new HashSet<CardModel>();
            TempShineSources[target] = sources;
        }

        sources.Add(cardSource);
    }

    public static void RegisterTempRevengeSource(Creature target, CardModel? cardSource)
    {
        if (cardSource == null)
        {
            return;
        }

        if (!TempRevengeSources.TryGetValue(target, out var sources))
        {
            sources = new HashSet<CardModel>();
            TempRevengeSources[target] = sources;
        }

        sources.Add(cardSource);
    }

    public static bool ShouldSkipTempShine(Creature target, CardModel currentCard)
    {
        if (!TempShineSources.TryGetValue(target, out var sources))
        {
            return false;
        }

        if (!sources.Remove(currentCard))
        {
            return false;
        }

        if (sources.Count == 0)
        {
            TempShineSources.Remove(target);
        }

        return true;
    }

    public static bool ShouldSkipTempRevenge(Creature target, CardModel currentCard)
    {
        if (!TempRevengeSources.TryGetValue(target, out var sources))
        {
            return false;
        }

        if (!sources.Remove(currentCard))
        {
            return false;
        }

        if (sources.Count == 0)
        {
            TempRevengeSources.Remove(target);
        }

        return true;
    }

    public static void Clear(Creature target)
    {
        TempShineSources.Remove(target);
        TempRevengeSources.Remove(target);
    }
}

// 闪耀值的统一规则入口。
public static class ShinePowerHelper
{
    // 总闪耀 = 永久 + 回合 + 临时。
    public static int GetTotalShine(Creature creature)
    {
        return creature.GetPowerAmount<ShinePower>()
             + creature.GetPowerAmount<TurnShinePower>()
             + creature.GetPowerAmount<TempShinePower>();
    }

    // 获得闪耀时不再抵消复仇值，直接添加对应层数。
    public static async Task ApplyShine(Creature target, decimal amount, ValueDuration duration, Creature? applier, CardModel? cardSource)
    {
        var value = (int)amount;
        if (value <= 0)
        {
            return;
        }

        switch (duration)
        {
            case ValueDuration.Permanent:
                await PowerCmd.Apply<ShinePower>(target, value, applier, cardSource);
                break;
            case ValueDuration.Turn:
                await PowerCmd.Apply<TurnShinePower>(target, value, applier, cardSource);
                break;
            case ValueDuration.Temp:
                TempPowerSourceTracker.RegisterTempShineSource(target, cardSource);
                await PowerCmd.Apply<TempShinePower>(target, value, applier, cardSource);
                break;
        }
    }

    public static async Task LoseShine(Creature target, int amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0)
        {
            return;
        }

        var remaining = amount;
        remaining = await ReducePower<TempShinePower>(target, remaining, applier, cardSource);
        remaining = await ReducePower<TurnShinePower>(target, remaining, applier, cardSource);
        await ReducePower<ShinePower>(target, remaining, applier, cardSource);
    }

    private static async Task<int> ReducePower<T>(Creature target, int amount, Creature? applier, CardModel? cardSource) where T : PowerModel
    {
        if (amount <= 0)
        {
            return 0;
        }

        var power = target.GetPower<T>();
        if (power == null)
        {
            return amount;
        }

        var remove = Math.Min(amount, power.Amount);
        if (remove <= 0)
        {
            return amount;
        }

        await PowerCmd.ModifyAmount(power, -remove, applier, cardSource);
        return amount - remove;
    }
}
