using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 获得8(11)点防御，获得1点临时闪耀。
public class DazzlingDefend : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    private const string CalculatedBlockKey = "CalculatedBlock";

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move),
        new ShineDymicVar(1m),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public DazzlingDefend() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        await ShinePowerHelper.ApplyShine(Owner.Creature,DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Temp, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}




