using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Oshinogo.Scripts.Powers;


public class LastMinuteStudyPower : CustomRubyPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        if (cardPlay.Card.Type != CardType.Skill)
        {
            return;
        }

        await ShinePowerHelper.ApplyShine(Owner, 1, ValueDuration.Temp, Owner, null);
    }
}
