using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 本回合内打出的消耗牌会进入弃牌堆。 谋划2
public class Expect : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => PlanCostHelper.CreatePlanCostHoverTips(2);

    public Expect() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 2);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 2))
        {
            return;
        }

        await PowerCmd.Apply<ExpectPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


