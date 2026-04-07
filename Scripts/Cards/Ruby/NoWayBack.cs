using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
public class NoWayBack : OshiCardModel
{
    public NoWayBack() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards;
        if (hand.Count == 0)
        {
            return;
        }

        var rng = Owner.RunState.Rng.CombatCardSelection;
        var selected = rng.NextItem(hand);
        if (selected == null)
        {
            return;
        }

        var cost = selected.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (cost < 0)
        {
            cost = 0;
        }

        selected.ExhaustOnNextPlay = true;
        await CardCmd.AutoPlay(choiceContext, selected, null);

        await RevengePowerHelper.ApplyRevenge(Owner.Creature, cost, ValueDuration.Temp, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
