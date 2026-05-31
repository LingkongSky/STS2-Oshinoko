using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 获得7(10)点格挡，下回合获得1点回合闪耀。

[RegisterCard(typeof(RubyCardPool))]
public class BackstageSupport : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
    ];

    public BackstageSupport() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<GainTurnShineNextTurnPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
        if (CombatHistoryHelper.HasGainedShineThisTurn(Owner))
        {
            await CreatureCmd.GainBlock(Owner.Creature, 2, ValueProp.Move, cardPlay);
        }

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}


