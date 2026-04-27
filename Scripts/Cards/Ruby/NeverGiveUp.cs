using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鑾峰緱2鐐归棯鑰€鍊硷紝骞惰幏寰?5鐐规牸鎸°€傝嫢鏈洖鍚堜綘浣跨敤杩囬棯鑰€鍊硷紝鑾峰緱1鐐硅兘閲忋€?

[Pool(typeof(RubyCardPool))]
public class NeverGiveUp : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
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
