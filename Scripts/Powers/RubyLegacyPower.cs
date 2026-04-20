using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinogo.Scripts.Powers;

public class RubyLegacyPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _lastRound;
    private CombatSide _lastSide;
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
            _firstShineTriggeredThisTurn = false;
        }

        if (power is ShinePower or TurnShinePower or TempShinePower)
        {
            await CreatureCmd.GainBlock(Owner, 3, ValueProp.Move, null);

            if (!_firstShineTriggeredThisTurn)
            {
                _firstShineTriggeredThisTurn = true;
                await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Temp, Owner, null);
            }
        }
        else if (power is RevengePower or TurnRevengePower or TempRevengePower)
        {
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
