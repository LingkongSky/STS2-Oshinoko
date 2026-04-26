using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 每次获得浸血花瓣，获得2点格挡。
public class Encore : AquaCardModel
{
    public Encore() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EncorePower>(Owner.Creature, 2, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
