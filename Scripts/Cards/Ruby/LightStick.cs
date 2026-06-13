using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

[RegisterCard(typeof(RubyCardPool))]
public class LightStick : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ShineDymicVar(1m),
        new HealVar(3m),
    ];

    public LightStick() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(
            Owner.Creature,
            DynamicVars[ShineDymicVar.Key].BaseValue,
            ValueDuration.Permanent,
            Owner.Creature,
            this);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
    }
}
