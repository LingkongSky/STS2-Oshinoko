using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 恢复3(5)点血量，获得20(25)金钱。
public class Embrace : AquaCardModel
{
    private const string CalculatedHealKey = "CalculatedHeal";
    private const string CalculatedGoldKey = "CalculatedGold";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword(), CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3),
        new GoldVar(20),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedHealKey, ShineValueType.Heal),
        ShineScaling.CreateCalculatedVar(CalculatedGoldKey, ShineValueType.Gold),
    ];

    public Embrace() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var heal = ShineScaling.Calculate(DynamicVars, CalculatedHealKey, cardPlay.Target);
        var gold = ShineScaling.Calculate(DynamicVars, CalculatedGoldKey, cardPlay.Target);
        await CreatureCmd.Heal(Owner.Creature, heal);
        await PlayerCmd.GainGold(gold, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2);
        DynamicVars.Gold.UpgradeValueBy(5);
    }
}



