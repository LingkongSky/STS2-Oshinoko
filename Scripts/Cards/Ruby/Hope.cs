using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby
{

    [Pool(typeof(RubyCardPool))]
    // 获得2点闪耀
    public class Hope : RubyCardModel
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

        public Hope() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self, true)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await ShinePowerHelper.ApplyShine(Owner.Creature, 2, ValueDuration.Permanent, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
