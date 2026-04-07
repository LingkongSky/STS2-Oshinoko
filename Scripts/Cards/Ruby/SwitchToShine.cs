using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
public class SwitchToShine : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public SwitchToShine() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = RevengePowerHelper.GetTotalRevenge(Owner.Creature);
        if (total <= 0)
        {
            return;
        }

        await RevengePowerHelper.LoseRevenge(Owner.Creature, total, Owner.Creature, this);
        await ShinePowerHelper.ApplyShine(Owner.Creature, total, ValueDuration.Permanent, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, total, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
