using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 查看抽牌堆上的5张牌，选择1张置于抽牌堆顶，下一回合额外获得2点能量和回合闪耀。

[RegisterCard(typeof(RubyCardPool))]
public class CarefulPlan : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public CarefulPlan() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var topCards = drawPile.Cards.Take(5).ToList();
        if (topCards.Count > 0)
        { var selected = topCards.FirstOrDefault();
            if (selected != null)
            {
                await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
            }
        }

        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this, true);
        await PowerCmd.Apply<GainTurnShineNextTurnPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



