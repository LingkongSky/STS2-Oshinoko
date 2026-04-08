using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得本场所获得过的闪耀值总量的格挡
[Pool(typeof(RubyCardPool))]
public class DailyAccumulation : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool GainsBlock => true;

    public DailyAccumulation() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ResourceUsageTracker.GetTotalShineGained(Owner);
        if (total > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, total, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
