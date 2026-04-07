using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
public class MothersLie : OshiCardModel
{
    public MothersLie() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var temp = Owner.Creature.GetPowerAmount<TempRevengePower>();
        if (temp <= 0)
        {
            return;
        }

        await RevengePowerHelper.LoseRevenge(Owner.Creature, temp, Owner.Creature, this);
        await RevengePowerHelper.ApplyRevenge(Owner.Creature, temp, ValueDuration.Permanent, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
