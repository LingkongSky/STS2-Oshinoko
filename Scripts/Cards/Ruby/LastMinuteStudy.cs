using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 姣忓洖鍚堢涓€娆℃墦鍑烘妧鑳界墝鏃讹紝鑾峰緱1鐐逛复鏃堕棯鑰€鍊笺€傝嫢杩欐槸鏈洖鍚堢3寮犳妧鑳界墝锛屾敼涓鸿幏寰?鐐瑰洖鍚堥棯鑰€鍊笺€?

[Pool(typeof(RubyCardPool))]
public class LastMinuteStudy : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    public LastMinuteStudy() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LastMinuteStudyPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
