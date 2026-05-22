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
public class ShinePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomIconPath => "res://Oshinogo/images/powers/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnShineChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }
}

// Turn shine: removed at end of turn.
public class TurnShinePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomIconPath => "res://Oshinogo/images/powers/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnShineChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// Temp shine: removed after the next Shine card is played.
public class TempShinePower : OshinogoCustomPower
{
    private readonly Dictionary<CardModel, int> _amountSnapshotBeforePlay = new();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomIconPath => "res://Oshinogo/images/powers/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player != null)
        {
            ResourceUsageTracker.OnShineChanged(Owner.Player, (int)amount);
        }

        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return Task.CompletedTask;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine.GetModKeywordCardKeyword()))
        {
            return Task.CompletedTask;
        }

        if (!_amountSnapshotBeforePlay.ContainsKey(cardPlay.Card))
        {
            _amountSnapshotBeforePlay[cardPlay.Card] = Amount;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine.GetModKeywordCardKeyword()))
        {
            return;
        }

        if (_amountSnapshotBeforePlay.TryGetValue(cardPlay.Card, out var snapshot))
        {
            _amountSnapshotBeforePlay.Remove(cardPlay.Card);
            var consume = Math.Min(snapshot, Amount);
            if (consume > 0)
            {
                await PowerCmd.ModifyAmount(context, this, -consume, Owner, cardPlay.Card);
            }
            return;
        }

        var preserve = TempPowerSourceTracker.PopTempShineSourceAmount(Owner, cardPlay.Card);
        await PowerCmd.Remove(this);
        if (preserve > 0)
        {
            await PowerCmd.Apply<TempShinePower>(context, Owner, preserve, Owner, cardPlay.Card);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            _amountSnapshotBeforePlay.Clear();
            TempPowerSourceTracker.Clear(Owner);
            await PowerCmd.Remove(this);
        }
    }
}

public class GainTempShineNextTurnPower : OshinogoCustomPower
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

public class GainTurnShineNextTurnPower : OshinogoCustomPower
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


public class NextShineDiscountPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Cards that actually received this discount and are allowed to consume one stack on play.
    private readonly HashSet<CardModel> _discountedCardsPendingConsume = new();

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner.Creature != Owner || !card.Keywords.Contains(OshinogoKeywords.Shine.GetModKeywordCardKeyword()))
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
        if (modifiedCost < originalCost)
        {
            _discountedCardsPendingConsume.Add(card);
        }
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine.GetModKeywordCardKeyword()))
        {
            return;
        }

        // Only consume when this played card actually got discounted by this power.
        if (!_discountedCardsPendingConsume.Remove(cardPlay.Card))
        {
            return;
        }

        if (Amount > 1)
        {
            await PowerCmd.ModifyAmount(context, this, -1, Owner, cardPlay.Card);
        }
        else
        {
            await PowerCmd.Remove(this);
        }
    }
}


