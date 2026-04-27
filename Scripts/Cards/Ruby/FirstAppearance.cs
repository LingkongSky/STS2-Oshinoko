using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鑾峰緱12(16)鐐规牸鎸★紝鍦ㄥ崱缁勯噷澧炲姞1寮犵湬鏅曘€?

[Pool(typeof(RubyCardPool))]
public class FirstAppearance : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine, CardKeyword.Exhaust];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
    ];

    public FirstAppearance() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner == null)
        {
            // Owner not set; skip.
            return;
        }

        if (owner.Creature == null)
        {
            // No creature bound yet; skip.
            return;
        }

        await CreatureCmd.GainBlock(owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        if (CardScope == null)
        {
            // No card scope available to create status card.
            return;
        }

        var stun = CardScope.CreateCard<Stunned>(owner!);
        if (stun != null)
        {
            await CardPileCmd.Add(stun, PileType.Draw);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}
