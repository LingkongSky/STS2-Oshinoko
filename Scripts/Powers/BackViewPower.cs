namespace Oshinoko.Scripts.Powers;


/// 下一张被打出的牌改为回到手中。
public class BackViewPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private readonly HashSet<CardModel> _pendingPowerClones = [];

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

        if (card.Type == CardType.Power)
        {
            _pendingPowerClones.Add(card);
            return (pileType, position);
        }

        if (pileType != PileType.Discard)
        {
            return (pileType, position);
        }

        return (PileType.Hand, CardPilePosition.Top);
    }

    public override async Task AfterModifyingCardPlayResultPileOrPosition(CardModel card, PileType pileType, CardPilePosition position)
    {
        if (card.Owner.Creature == Owner && !_pendingPowerClones.Contains(card))
        {
            Flash();
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!_pendingPowerClones.Remove(cardPlay.Card))
        {
            return;
        }

        if (cardPlay.Card.Owner.Creature != Owner || Owner.Player == null)
        {
            return;
        }

        var clone = cardPlay.Card.CreateClone();
        await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, Owner.Player, CardPilePosition.Top);
        Flash();
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            _pendingPowerClones.Clear();
            await PowerCmd.Remove(this);
        }
    }
}
