namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
//  
public class Watch : AquaCardModel
{
    private const string EscapeKey = "WatchEscape";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(EscapeKey, 2)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => PlanAndKeywordTips(1, "ESCAPE");
    public Watch() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EscapePower>(choiceContext, Owner.Creature, DynamicVars[EscapeKey].BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[EscapeKey].UpgradeValueBy(1);
    }
}


