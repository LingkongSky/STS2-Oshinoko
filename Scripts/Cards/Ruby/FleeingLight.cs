using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ÿʹ��5(4)�㸴�𣬳�1���ơ�

[RegisterCard(typeof(RubyCardPool))]
public class FleeingLight : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    private const string ThresholdKey = "Threshold";
    private const int DefaultThreshold = 5;
    private const int UpgradedThreshold = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar(ThresholdKey, 5),
        ];

    public FleeingLight() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FleeingLightPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ThresholdKey].UpgradeValueBy(-1);
    }
}


