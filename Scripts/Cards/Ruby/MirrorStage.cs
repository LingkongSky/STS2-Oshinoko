using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得14(18)点格挡，本回合反弹所有被格挡的伤害。

[RegisterCard(typeof(RubyCardPool))]
public class MirrorStage : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine.GetModKeywordCardKeyword()];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(14m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar("CalculatedBlock", ShineValueType.Block),
    ];

    public MirrorStage() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Use calculated block so shine bonuses apply.
        var blockValue = ShineScaling.Calculate(DynamicVars, "CalculatedBlock", Owner.Creature);
        await CreatureCmd.GainBlock(Owner.Creature, blockValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<MirrorStagePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}



