
namespace Oshinoko.Scripts.Cards.Ruby
{
    [RegisterCard(typeof(RubyCardPool))]
    /// 描述: 获得4(5)点回合复仇。保留。
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



