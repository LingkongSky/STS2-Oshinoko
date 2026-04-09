using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

// Value duration types: permanent, turn, temp.
public enum ValueDuration
{
    Permanent,
    Turn,
    Temp,
}

// Track temp resource sources so we can preserve only the amount granted by the current card.
public static class TempPowerSourceTracker
{
    private static readonly Dictionary<Creature, Dictionary<CardModel, int>> TempShineSources = new();

    private static readonly Dictionary<Creature, Dictionary<CardModel, int>> TempRevengeSources = new();

    public static void RegisterTempShineSource(Creature target, CardModel? cardSource, int amount)
    {
        if (cardSource == null || amount <= 0)
        {
            return;
        }

        if (!TempShineSources.TryGetValue(target, out var sources))
        {
            sources = new Dictionary<CardModel, int>();
            TempShineSources[target] = sources;
        }

        if (sources.TryGetValue(cardSource, out var existing))
        {
            sources[cardSource] = existing + amount;
            return;
        }

        sources[cardSource] = amount;
    }

    public static void RegisterTempRevengeSource(Creature target, CardModel? cardSource, int amount)
    {
        if (cardSource == null || amount <= 0)
        {
            return;
        }

        if (!TempRevengeSources.TryGetValue(target, out var sources))
        {
            sources = new Dictionary<CardModel, int>();
            TempRevengeSources[target] = sources;
        }

        if (sources.TryGetValue(cardSource, out var existing))
        {
            sources[cardSource] = existing + amount;
            return;
        }

        sources[cardSource] = amount;
    }

    public static int PopTempShineSourceAmount(Creature target, CardModel currentCard)
    {
        if (!TempShineSources.TryGetValue(target, out var sources))
        {
            return 0;
        }

        if (!sources.TryGetValue(currentCard, out var amount))
        {
            return 0;
        }

        sources.Remove(currentCard);
        if (sources.Count == 0)
        {
            TempShineSources.Remove(target);
        }

        return amount;
    }

    public static int PopTempRevengeSourceAmount(Creature target, CardModel currentCard)
    {
        if (!TempRevengeSources.TryGetValue(target, out var sources))
        {
            return 0;
        }

        if (!sources.TryGetValue(currentCard, out var amount))
        {
            return 0;
        }

        sources.Remove(currentCard);
        if (sources.Count == 0)
        {
            TempRevengeSources.Remove(target);
        }

        return amount;
    }

    public static void Clear(Creature target)
    {
        TempShineSources.Remove(target);
        TempRevengeSources.Remove(target);
    }
}

// Unified shine rules.
public static class ShinePowerHelper
{
    // Total shine = permanent + turn + temp.
    public static int GetTotalShine(Creature creature)
    {
        return creature.GetPowerAmount<ShinePower>()
             + creature.GetPowerAmount<TurnShinePower>()
             + creature.GetPowerAmount<TempShinePower>();
    }

    // Gaining shine no longer offsets revenge; it stacks directly.
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
                TempPowerSourceTracker.RegisterTempShineSource(target, cardSource, value);
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
