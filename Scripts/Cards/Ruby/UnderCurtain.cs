using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby
{
    [Pool(typeof(RubyCardPool))]
    // 鑾峰緱4(5)鐐逛复鏃跺浠?
    public class UnderCurtain : RubyCardModel
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

        private const string ThresholdKey = "Threshold";

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
        new DynamicVar(ThresholdKey, 4)
        ];


        public UnderCurtain() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self, true)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await RevengePowerHelper.ApplyRevenge(Owner.Creature, DynamicVars[ThresholdKey].BaseValue, ValueDuration.Temp, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars[ThresholdKey].UpgradeValueBy(1);
        }
    }
}
