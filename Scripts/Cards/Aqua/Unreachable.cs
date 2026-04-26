using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 每抽6张牌，额外抽一张牌。
public class Unreachable : AquaCardModel
{
    public Unreachable() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<UnreachablePower>(Owner.Creature, 6, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
