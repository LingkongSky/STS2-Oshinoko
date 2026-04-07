using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

public class GainTempShineNextTurnPower : CustomPowerModel
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

public class GainTurnShineNextTurnPower : CustomPowerModel
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

public class GainTempRevengeNextTurnPower : CustomPowerModel
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

public class GainTurnRevengeNextTurnPower : CustomPowerModel
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

public class VengeanceBellNextTurnPower : CustomPowerModel
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
        for (int i = 0; i < Amount; i++)
        {
            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                opponents,
                3,
                ValueProp.Move,
                Owner,
                null
            );
        }

        await PowerCmd.Remove(this);
    }
}

public class ActCutePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
        {
            return 1m;
        }

        if (!props.IsPoweredAttack_())
        {
            return 1m;
        }

        return 0.75m;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public class ChasingLightPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _spent;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner)
        {
            return;
        }

        if (amount >= 0)
        {
            return;
        }

        if (power is not ShinePower and not TurnShinePower and not TempShinePower)
        {
            return;
        }

        _spent += (int)(-amount);
        var threshold = Math.Max(1, Amount);
        var triggers = _spent / threshold;
        if (triggers <= 0)
        {
            return;
        }

        _spent -= triggers * threshold;
        await PlayerCmd.GainEnergy(triggers, Owner.Player);
    }
}

public class FleeingLightPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _spent;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner)
        {
            return;
        }

        if (amount >= 0)
        {
            return;
        }

        if (power is not RevengePower and not TurnRevengePower and not TempRevengePower)
        {
            return;
        }

        _spent += (int)(-amount);
        var threshold = Math.Max(1, Amount);
        var triggers = _spent / threshold;
        if (triggers <= 0)
        {
            return;
        }

        _spent -= triggers * threshold;
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), triggers, Owner.Player);
    }
}

public class LastMinuteStudyPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player)
        {
            await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Temp, Owner, null);
        }
    }
}

public class StayIndoorsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player)
        {
            await RevengePowerHelper.ApplyRevenge(Owner, 1, ValueDuration.Temp, Owner, null);
        }
    }
}

public class NotAsFragileAsImaginedPower : CustomPowerModel
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

public class RumorNetworkPower : CustomPowerModel
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
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
    }
}

public class StageArmorPower : CustomPowerModel
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

public class RubyLegacyPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

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

        if (power is ShinePower or TurnShinePower or TempShinePower)
        {
            await CreatureCmd.GainBlock(Owner, amount, ValueProp.Move, null);
            return;
        }

        if (power is RevengePower or TurnRevengePower or TempRevengePower)
        {
            if (!ResourceUsageTracker.TryTriggerRevengeGainThisTurn(Owner.Player))
            {
                return;
            }

            await CreatureCmd.Damage(
                new BlockingPlayerChoiceContext(),
                Owner,
                1,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                Owner
            );
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }
}

public class DualMirrorPower : CustomPowerModel
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

        if (!ResourceUsageTracker.TryTriggerFirstChangeThisTurn(Owner.Player))
        {
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
        await PlayerCmd.GainEnergy(1, Owner.Player);
    }
}

public class ShellForgedByLiesPower : CustomPowerModel
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

        var damage = revenge * 5;
        var opponents = combatState.GetOpponentsOf(Owner).ToList();
        await CreatureCmd.Damage(choiceContext, opponents, damage, ValueProp.Move, Owner, null);
    }
}

public class LightFromPassionPower : CustomPowerModel
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

        var block = shine * 5;
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Move, null);
    }
}

public class RetreatBackstagePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

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

        await PlayerCmd.GainEnergy(amount, Owner.Player);
    }
}

public class TakeTheStagePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

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

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), amount, Owner.Player);
    }
}
