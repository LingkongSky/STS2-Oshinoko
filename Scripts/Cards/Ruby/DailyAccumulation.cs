using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得本场获得过的闪耀与复仇总和的格挡。

[RegisterCard(typeof(RubyCardPool))]
public class DailyAccumulation : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE", "REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool GainsBlock => true;

    public DailyAccumulation() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ResourceUsageTracker.GetTotalShineGained(Owner)
            + ResourceUsageTracker.GetTotalRevengeGained(Owner);
        if (total > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, total, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



