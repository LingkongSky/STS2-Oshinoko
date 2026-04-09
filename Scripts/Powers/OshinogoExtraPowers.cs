using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

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

public class GainTempRevengeNextTurnPower : CustomRubyPower
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

public class GainTurnRevengeNextTurnPower : CustomRubyPower
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

public class VengeanceBellNextTurnPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            await PowerCmd.Remove(this);
            return;
        }

        var opponents = combatState.GetOpponentsOf(Owner).ToList();

        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), opponents, 10, ValueProp.Move, Owner, null);

        await PowerCmd.Remove(this);
    }
}

public class ActCuteNextTurnPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await PowerCmd.Apply<IntangiblePower>(Owner, 1, Owner, null);
        await PowerCmd.Apply<ActCuteLockoutPower>(Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public class ActCuteLockoutPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature != Owner)
        {
            return true;
        }

        return false;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            // Use engine heal signature (creature, amount).
            await CreatureCmd.Heal(Owner, Amount);
            await PowerCmd.Remove(this);
        }
    }
}

public class MirrorStagePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
        {
            return;
        }

        if (dealer == null)
        {
            return;
        }

        if (result.BlockedDamage <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, dealer, result.BlockedDamage, ValueProp.Move, Owner, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public class MothersLiePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
        {
            return;
        }

        if (result.UnblockedDamage <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var opponents = combatState.GetOpponentsOf(Owner).ToList();
        await CreatureCmd.Damage(choiceContext, opponents, Amount, ValueProp.Move, Owner, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public class NextShineDiscountPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private CardModel? _sourceCard;
    private bool _skipRemovalForSource;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _sourceCard = cardSource;
        _skipRemovalForSource = true;
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

        modifiedCost = Math.Max(0, originalCost - 1);
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

        if (_skipRemovalForSource && _sourceCard != null && ReferenceEquals(cardPlay.Card, _sourceCard))
        {
            _skipRemovalForSource = false;
            return;
        }

        _skipRemovalForSource = false;
        await PowerCmd.Remove(this);
    }
}

public class ChasingLightPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _spent;

    public override int DisplayAmount => Amount - _spent % Amount;


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var usedShine = ShineScaling.GetShineUsedByCard(cardPlay.Card);
        if (usedShine <= 0)
        {
            return;
        }

        _spent += usedShine;
        InvokeDisplayAmountChanged();

        if (_spent < Amount)
        {
            return;
        }

        var triggers = (int)Math.Floor((decimal)_spent / Amount);
        _spent = _spent % triggers;

        /*
        var threshold = Math.Max(1, Amount);
        var triggers = _spent / threshold;
        if (triggers <= 0)
        {
            return;
        }

        _spent -= triggers * threshold;
        if (Owner.Player == null)
        {
            // No player to draw for.
            return;
        }
        */
        await PlayerCmd.GainEnergy(triggers, Owner.Player);
        InvokeDisplayAmountChanged();

    }

}

public class FleeingLightPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _spent;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var usedRevenge = ShineScaling.GetRevengeUsedByCard(cardPlay.Card);
        if (usedRevenge <= 0)
        {
            return;
        }

        _spent += usedRevenge;
        InvokeDisplayAmountChanged();

        if (_spent < Amount)
        {
            return;
        }

        var triggers = (int)Math.Floor((decimal)_spent / Amount);
        _spent = _spent % triggers;


        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), triggers, Owner.Player);
        InvokeDisplayAmountChanged();

    }

}

public class LastMinuteStudyPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _lastRound;
    private CombatSide _lastSide;
    private int _skillsPlayedThisTurn;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _skillsPlayedThisTurn = 0;
        }

        if (cardPlay.Card.Type != CardType.Skill)
        {
            return;
        }

        _skillsPlayedThisTurn++;
        if (_skillsPlayedThisTurn == 1)
        {
            await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Temp, Owner, null);
        }
        else if (_skillsPlayedThisTurn == 3)
        {
            await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Turn, Owner, null);
        }
    }
}

public class StayIndoorsPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        if (!CombatHistoryHelper.HasLostHpThisTurn(Owner.Player))
        {
            return;
        }

        await RevengePowerHelper.ApplyRevenge(Owner, 1, ValueDuration.Temp, Owner, null);
        await CreatureCmd.GainBlock(Owner, 3, ValueProp.Move, null);
    }
}

public class NotAsFragileAsImaginedPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if (target == Owner && canonicalPower is FrailPower)
        {
            modifiedAmount = 0m;
            return true;
        }

        return base.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var frail = Owner.GetPower<FrailPower>();
        if (frail != null)
        {
            await PowerCmd.Remove(frail);
        }
    }
}

public class RumorNetworkPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
    private bool _triggeredThisTurn;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _triggeredThisTurn = false;
        }

        if (_triggeredThisTurn)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        if (power is not WeakPower and not VulnerablePower)
        {
            return;
        }

        if (applier != Owner)
        {
            return;
        }

        _triggeredThisTurn = true;
        var drawCount = Math.Max(1, (int)Amount);
        if (Owner.Player == null)
        {
            // No player to draw for.
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), drawCount, Owner.Player);
    }
}

public class StageArmorPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
        {
            return;
        }

        if (result.UnblockedDamage <= 0)
        {
            return;
        }

        var blockAmount = result.UnblockedDamage * Amount;
        await CreatureCmd.GainBlock(Owner, blockAmount, ValueProp.Move, null);
    }
}

public class DieAlonePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
    private bool _triggeredThisTurn;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
        {
            return;
        }

        if (result.UnblockedDamage <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _triggeredThisTurn = false;
        }

        if (_triggeredThisTurn)
        {
            return;
        }

        _triggeredThisTurn = true;
        await CreatureCmd.GainBlock(Owner, 3, ValueProp.Move, null);
    }
}

public class RubyLegacyPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
    private bool _gainedShineThisTurn;
    private bool _gainedRevengeThisTurn;
    private bool _drewThisTurn;
    private bool _firstShineTriggeredThisTurn;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _gainedShineThisTurn = false;
            _gainedRevengeThisTurn = false;
            _drewThisTurn = false;
            _firstShineTriggeredThisTurn = false;
        }

        if (power is ShinePower or TurnShinePower or TempShinePower)
        {
            _gainedShineThisTurn = true;
            await CreatureCmd.GainBlock(Owner, 3, ValueProp.Move, null);

            if (!_firstShineTriggeredThisTurn)
            {
                _firstShineTriggeredThisTurn = true;
                await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Temp, Owner, null);
            }
        }
        else if (power is RevengePower or TurnRevengePower or TempRevengePower)
        {
            _gainedRevengeThisTurn = true;
            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                Owner,
                1,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                Owner
            );
            await CreatureCmd.GainBlock(Owner, 8, ValueProp.Move, null);
        }

    }
}

public class DualMirrorPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner || amount == 0)
        {
            return;
        }

        if (power is not ShinePower and not TurnShinePower and not TempShinePower
            && power is not RevengePower and not TurnRevengePower and not TempRevengePower)
        {
            return;
        }

        if (Owner.Player == null)
        {
            // No player to draw for.
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
    }
}

public class ShellForgedByLiesPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var revenge = RevengePowerHelper.GetTotalRevenge(Owner);
        if (revenge <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var damage = revenge * 4;
        if (CombatHistoryHelper.HasLostHpThisTurn(Owner.Player))
        {
            damage += 2;
        }
        var opponents = combatState.GetOpponentsOf(Owner).ToList();
        await CreatureCmd.Damage(choiceContext, opponents, damage, ValueProp.Move, Owner, null);
    }
}

public class LightFromPassionPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var shine = ShinePowerHelper.GetTotalShine(Owner);
        if (shine <= 0)
        {
            return;
        }

        var block = shine * 4;
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Move, null);
    }
}

public class RetreatBackstagePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
    private bool _triggeredThisTurn;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner || amount <= 0)
        {
            return;
        }

        if (power is not RevengePower and not TurnRevengePower and not TempRevengePower)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _triggeredThisTurn = false;
        }

        if (_triggeredThisTurn)
        {
            return;
        }

        _triggeredThisTurn = true;
        if (Owner.Player == null)
        {
            // No player to grant energy to.
            return;
        }

        await PlayerCmd.GainEnergy(1, Owner.Player);
        await CreatureCmd.GainBlock(Owner, 2, ValueProp.Move, null);
    }
}

public class TakeTheStagePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
    private bool _triggeredThisTurn;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner || amount <= 0)
        {
            return;
        }

        if (power is not ShinePower and not TurnShinePower and not TempShinePower)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _triggeredThisTurn = false;
        }

        if (_triggeredThisTurn)
        {
            return;
        }

        _triggeredThisTurn = true;
        if (Owner.Player == null)
        {
            // No player to draw for.
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
        await CreatureCmd.GainBlock(Owner, 2, ValueProp.Move, null);
    }
}

public class IdolRadiancePower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
    private bool _triggeredThisTurn;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (_lastRound != combatState.RoundNumber || _lastSide != combatState.CurrentSide)
        {
            _lastRound = combatState.RoundNumber;
            _lastSide = combatState.CurrentSide;
            _triggeredThisTurn = false;
        }

        if (_triggeredThisTurn)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        _triggeredThisTurn = true;
        await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Temp, Owner, null);
    }
}

public class RevealTruthPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        if (Owner.MaxHp <= 0)
        {
            return;
        }

        // Use current HP for half-health check.
        if (Owner.CurrentHp < Owner.MaxHp / 2m)
        {
            await RevengePowerHelper.ApplyRevenge(Owner, 1, ValueDuration.Permanent, Owner, null);
        }
    }
}
