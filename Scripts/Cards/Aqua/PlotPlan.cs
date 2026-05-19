using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 抽3(4)张牌。 谋划1
public class PlotPlan : AquaCardModel
{
    private const string CalculatedCardsKey = "CalculatedCards";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedCardsKey, ShineValueType.Cards),
    ];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => PlanCostHelper.CreatePlanCostHoverTips(1);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine.GetModKeywordCardKeyword()];

    public PlotPlan() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 1))
        {
            return;
        }

        var finalCards = ShineScaling.Calculate(DynamicVars, CalculatedCardsKey, cardPlay.Target);
        await CardPileCmd.Draw(choiceContext, finalCards, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}



