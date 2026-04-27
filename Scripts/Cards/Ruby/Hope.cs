using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby
{

    [Pool(typeof(RubyCardPool))]
    // 鑾峰緱2鐐归棯鑰€
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
