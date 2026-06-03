using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ���1(2)����ʱ��ҫ����1(2)���ơ�

[RegisterCard(typeof(RubyCardPool))]
public class AquaCall : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    private const string ThresholdKey = "Threshold";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ShineDymicVar(1m),
        new DynamicVar(ThresholdKey, 1)
    ];

    public AquaCall() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Temp, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars[ThresholdKey].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ShineDymicVar.Key].UpgradeValueBy(1);
        DynamicVars[ThresholdKey].UpgradeValueBy(1);
    }
}


