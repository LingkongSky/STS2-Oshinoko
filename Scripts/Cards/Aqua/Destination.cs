
namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 获得5(10)点复仇，5(10)点力量。
public class Destination : AquaCardModel
{
    private const string StrengthKey = "DestinationStrength";

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RevengeDynamicVar(5m),
        new DynamicVar(StrengthKey, 5),
    ];

    public Destination() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await RevengePowerHelper.ApplyRevenge(Owner.Creature, DynamicVars[RevengeDynamicVar.Key].BaseValue, ValueDuration.Permanent, Owner.Creature, this);
        await PowerCmd.Apply<TemporaryStrengthPower>(choiceContext, Owner.Creature, DynamicVars[StrengthKey].BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[RevengeDynamicVar.Key].UpgradeValueBy(5);
        DynamicVars[StrengthKey].UpgradeValueBy(5);
    }
}



