namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 战斗结束从卡组中移除一张牌 谋划1

public class HandscrollPainting : AquaCardModel
{
    public HandscrollPainting() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HandscrollPaintingPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


