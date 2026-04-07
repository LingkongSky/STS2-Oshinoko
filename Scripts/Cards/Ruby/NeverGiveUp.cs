using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
public class NeverGiveUp : OshiCardModel
{
    public NeverGiveUp() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var temp = Owner.Creature.GetPowerAmount<TempShinePower>();
        if (temp <= 0)
        {
            return;
        }

        await ShinePowerHelper.LoseShine(Owner.Creature, temp, Owner.Creature, this);
        await ShinePowerHelper.ApplyShine(Owner.Creature, temp, ValueDuration.Permanent, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
