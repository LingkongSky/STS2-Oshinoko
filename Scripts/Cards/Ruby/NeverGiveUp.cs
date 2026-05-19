using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得2点闪耀，并获得15点格挡。若本回合你使用过闪耀，获得1点能量。

[RegisterCard(typeof(RubyCardPool))]
public class NeverGiveUp : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public NeverGiveUp() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, 2, ValueDuration.Permanent, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, 15, ValueProp.Move, cardPlay);
        if (CombatHistoryHelper.HasPlayedShineCardWithShineThisTurn(Owner))
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



