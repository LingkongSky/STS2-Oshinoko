namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得6(9)点格挡。在弃牌堆中选择一张置入抽牌堆顶部。

[RegisterCard(typeof(RubyCardPool))]
public class SmallTrick : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine.GetModKeywordCardKeyword()];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
    ];

    public SmallTrick() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay); CardPile pile = PileType.Discard.GetPile(base.Owner);
        CardModel? cardModel = pile.Cards.FirstOrDefault();
        if (cardModel != null)
        {
            await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}





