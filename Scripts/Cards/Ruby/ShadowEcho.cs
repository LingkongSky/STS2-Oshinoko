using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鑾峰緱2鐐瑰浠囧€笺€傝嫢褰撳墠澶嶄粐鍊煎ぇ浜?锛屾娊2寮犵墝銆?


[Pool(typeof(RubyCardPool))]
public class ShadowEcho : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("REVENGE");
    protected override IEnumerable<DynamicVar> CanonicalVars => [new RevengeDynamicVar(2m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public ShadowEcho() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await RevengePowerHelper.ApplyRevenge(Owner.Creature, DynamicVars[RevengeDynamicVar.Key].BaseValue, ValueDuration.Permanent, Owner.Creature, this);
        if (RevengePowerHelper.GetTotalRevenge(Owner.Creature) > 3)
        {
            await CardPileCmd.Draw(choiceContext, 2, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
