namespace Oshinoko.Scripts.Powers;

public class ProfilePower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return;
        }

        if (!PlanCostHelper.ConsumePlanConsumedMark(cardPlay.Card))
        {
            return;
        }

        if (Owner.Player == null)
        {
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
    }
}
