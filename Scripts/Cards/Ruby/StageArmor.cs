using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 姣忓け鍘?鐐圭敓鍛斤紝鑾峰緱2(3)鐐规牸鎸°€?

[Pool(typeof(RubyCardPool))]
public class StageArmor : RubyCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockPerHp", 2)];

    public StageArmor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StageArmorPower>(Owner.Creature, DynamicVars["BlockPerHp"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerHp"].UpgradeValueBy(1);
    }
}
