using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Oshinogo.Scripts.Powers;


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
