
namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 每使用6(5)点闪耀，获得1点能量。

[RegisterCard(typeof(RubyCardPool))]
public class ChasingLight : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    private const string ThresholdKey = "Threshold";
    private const int DefaultThreshold = 6;
    private const int UpgradedThreshold = 5;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(ThresholdKey, 6)];

    public ChasingLight() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var threshold = IsUpgraded ? UpgradedThreshold : DefaultThreshold;
        await PowerCmd.Apply<ChasingLightPower>(choiceContext, Owner.Creature, threshold, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ThresholdKey].UpgradeValueBy(-1);
    }
}


