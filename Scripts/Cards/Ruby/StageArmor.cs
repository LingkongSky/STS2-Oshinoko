using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 每失去1点生命，获得3(4)点格挡
[Pool(typeof(RubyCardPool))]
public class StageArmor : OshiCardModel
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
