using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

// Permanent revenge: persists and triggers HP loss when a Shine card is played.
public class RevengePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ruby_energy_black.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big_black.png";

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnRevengeChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await RevengePowerHelper.TriggerShineCardCost(context, this, cardPlay);
    }
}

// Turn revenge: expires at end of turn.
public class TurnRevengePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ruby_energy_black.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big_black.png";

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnRevengeChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await RevengePowerHelper.TriggerShineCardCost(context, this, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// Temp revenge: removed after the next Shine card is played.
public class TempRevengePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ruby_energy_black.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big_black.png";

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnRevengeChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        await RevengePowerHelper.TriggerShineCardCost(context, this, cardPlay);

        var preserve = TempPowerSourceTracker.PopTempRevengeSourceAmount(Owner, cardPlay.Card);
        await PowerCmd.Remove(this);
        if (preserve > 0)
        {
            await PowerCmd.Apply<TempRevengePower>(Owner, preserve, Owner, cardPlay.Card);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            TempPowerSourceTracker.Clear(Owner);
            await PowerCmd.Remove(this);
        }
    }
}

// Unified revenge rules.
public static class RevengePowerHelper
{
    // Total revenge = permanent + turn + temp.
    public static int GetTotalRevenge(Creature creature)
    {
        return creature.GetPowerAmount<RevengePower>()
             + creature.GetPowerAmount<TurnRevengePower>()
             + creature.GetPowerAmount<TempRevengePower>();
    }

    // Gaining revenge no longer offsets shine; it stacks directly.
    public static async Task ApplyRevenge(Creature target, decimal amount, ValueDuration duration, Creature? applier, CardModel? cardSource)
    {
        var value = (int)amount;
        if (value <= 0)
        {
            return;
        }

        switch (duration)
        {
            case ValueDuration.Permanent:
                await PowerCmd.Apply<RevengePower>(target, value, applier, cardSource);
                break;
            case ValueDuration.Turn:
                await PowerCmd.Apply<TurnRevengePower>(target, value, applier, cardSource);
                break;
            case ValueDuration.Temp:
                TempPowerSourceTracker.RegisterTempRevengeSource(target, cardSource, value);
                await PowerCmd.Apply<TempRevengePower>(target, value, applier, cardSource);
                break;
        }
    }

    public static async Task LoseRevenge(Creature target, int amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0)
        {
            return;
        }

        var remaining = amount;
        remaining = await ReducePower<TempRevengePower>(target, remaining, applier, cardSource);
        remaining = await ReducePower<TurnRevengePower>(target, remaining, applier, cardSource);
        await ReducePower<RevengePower>(target, remaining, applier, cardSource);
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

    // Ensure only one revenge source handles loss to avoid double-triggering.
    public static bool ShouldHandleLoss(PowerModel power)
    {
        if (power.Owner.HasPower<RevengePower>())
        {
            return power is RevengePower;
        }

        if (power.Owner.HasPower<TurnRevengePower>())
        {
            return power is TurnRevengePower;
        }

        return power is TempRevengePower;
    }

    // On playing a Shine card, lose HP equal to total revenge.
    public static async Task TriggerShineCardCost(PlayerChoiceContext context, PowerModel power, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != power.Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        if (!ShouldHandleLoss(power))
        {
            return;
        }

        var revenge = GetTotalRevenge(power.Owner);
        if (revenge <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(
            context,
            power.Owner,
            revenge,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            power.Owner
        );
    }
}



public class GainTempRevengeNextTurnPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await RevengePowerHelper.ApplyRevenge(Owner, Amount, ValueDuration.Temp, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public class GainTurnRevengeNextTurnPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await RevengePowerHelper.ApplyRevenge(Owner, Amount, ValueDuration.Turn, Owner, null);
        await PowerCmd.Remove(this);
    }
}
