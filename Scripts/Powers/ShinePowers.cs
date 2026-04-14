using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

// Permanent shine.
public class ShinePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnShineChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }
}

// Turn shine: removed at end of turn.
public class TurnShinePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnShineChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// Temp shine: removed after the next Shine card is played.
public class TempShinePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnShineChanged(Owner.Player, (int)amount);
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

        var preserve = TempPowerSourceTracker.PopTempShineSourceAmount(Owner, cardPlay.Card);
        await PowerCmd.Remove(this);
        if (preserve > 0)
        {
            await PowerCmd.Apply<TempShinePower>(Owner, preserve, Owner, cardPlay.Card);
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

public class GainTempShineNextTurnPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await ShinePowerHelper.ApplyShine(Owner, Amount, ValueDuration.Temp, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public class GainTurnShineNextTurnPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await ShinePowerHelper.ApplyShine(Owner, Amount, ValueDuration.Turn, Owner, null);
        await PowerCmd.Remove(this);
    }
}


public class NextShineDiscountPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel? _sourceCard;
    private int _pendingSkips;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _sourceCard = cardSource;
        _pendingSkips += 1;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner.Creature != Owner || !card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            modifiedCost = originalCost;
            return false;
        }

        if (originalCost <= 0)
        {
            modifiedCost = originalCost;
            return false;
        }

        modifiedCost = Math.Max(0, originalCost - Amount);
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        if (_pendingSkips > 0 && _sourceCard != null && ReferenceEquals(cardPlay.Card, _sourceCard))
        {
            _pendingSkips--;
            return;
        }

        if (Amount > 1)
        {
            await PowerCmd.ModifyAmount(this, -1, Owner, cardPlay.Card);
        }
        else
        {
            await PowerCmd.Remove(this);
        }
    }
}
