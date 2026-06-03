namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 热情：获得一点闪耀
public class Passion : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(1)];

    public Passion() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Permanent, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}




