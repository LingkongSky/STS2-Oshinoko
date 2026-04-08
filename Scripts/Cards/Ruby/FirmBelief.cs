using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 将闪耀值减半后转换为永久闪耀
[Pool(typeof(RubyCardPool))]
public class FirmBelief : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FirmBelief() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ShinePowerHelper.GetTotalShine(Owner.Creature);
        if (total <= 0)
        {
            return;
        }

        var converted = total / 2;
        await ShinePowerHelper.LoseShine(Owner.Creature, total, Owner.Creature, this);
        if (converted > 0)
        {
            await ShinePowerHelper.ApplyShine(Owner.Creature, converted, ValueDuration.Permanent, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
