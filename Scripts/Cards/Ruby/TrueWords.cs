using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鍘婚櫎涓€鍚嶆晫浜虹殑鎵€鏈夐槻寰°€傝嫢鍏堕槻寰″ぇ浜?5锛屽垯鑾峰緱1鐐归棯鑰€鍊煎苟鎶?寮犵墝銆?

[Pool(typeof(RubyCardPool))]
public class TrueWords : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public TrueWords() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var block = cardPlay.Target.Block;
        if (block > 0)
        {
            await CreatureCmd.LoseBlock(cardPlay.Target, block);
        }

        if (block > 15)
        {
            await ShinePowerHelper.ApplyShine(Owner.Creature, 1, ValueDuration.Permanent, Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
