using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鍥炲悎缁撴潫鏃跺鎵€鏈夋晫浜洪€犳垚澶嶄粐鍊?4鐨勪激瀹炽€傝嫢鏈洖鍚堜綘澶卞幓杩囩敓鍛斤紝浼ゅ+8銆?

[Pool(typeof(RubyCardPool))]
public class ShellForgedByLies : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("REVENGE");
    public ShellForgedByLies() : base(2, CardType.Power, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ShellForgedByLiesPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
