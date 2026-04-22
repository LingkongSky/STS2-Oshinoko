using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinogo.Scripts.Powers;

public class RetreatBackstagePower : OshinogoCustomPower
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

        await PlayerCmd.GainEnergy(2, Owner.Player);
        await CreatureCmd.GainBlock(Owner, 10, ValueProp.Move, null);
    }
}
