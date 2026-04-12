using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Oshinogo.Scripts.Powers;

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
