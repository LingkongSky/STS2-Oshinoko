namespace Oshinogo.Scripts.Powers;


/// 下一张被打出的牌改为回到手中。
public class BackViewPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        PileType pileType,
        CardPilePosition position
    )
    {
        if (card.Owner.Creature != Owner)
        {
            return (pileType, position);
        }

        return (PileType.Hand, CardPilePosition.Top);
    }

    public override async Task AfterModifyingCardPlayResultPileOrPosition(CardModel card, PileType pileType, CardPilePosition position)
    {
        if (card.Owner.Creature == Owner && pileType == PileType.Hand)
        {
            Flash();
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
