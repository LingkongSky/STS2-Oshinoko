using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

public class IdolRadiancePower : OshinogoCustomPower
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
        await ShinePowerHelper.ApplyShine(Owner, 2, ValueDuration.Temp, Owner, null);
    }
}
