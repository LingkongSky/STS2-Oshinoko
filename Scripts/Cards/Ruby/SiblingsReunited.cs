using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 将你的复仇全部转换为闪耀。抽等量卡牌，并获得20点格挡。

[RegisterCard(typeof(RubyCardPool))]
public class SiblingsReunited : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE", "REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
[
    new BlockVar(20m, ValueProp.Move),
    ];

    public SiblingsReunited() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = RevengePowerHelper.GetTotalRevenge(Owner.Creature);
        if (total <= 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
            return;
        }

        await RevengePowerHelper.LoseRevenge(Owner.Creature, total, Owner.Creature, this);
        await ShinePowerHelper.ApplyShine(Owner.Creature, total, ValueDuration.Permanent, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, total, Owner);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



