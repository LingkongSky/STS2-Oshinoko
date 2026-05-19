namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 所有队友回复3(5)点生命，你额外回复2点生命。

[RegisterCard(typeof(RubyCardPool))]
public class ActSpoiled : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, OshinogoKeywords.Shine.GetModKeywordCardKeyword()];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3m),
    ];

    public ActSpoiled() : base(2, CardType.Skill, CardRarity.Event, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState != null)
        {
            var teammates = combatState.GetTeammatesOf(Owner.Creature);
            foreach (var teammate in teammates)
            {
                if (teammate == Owner.Creature)
                {
                    continue;
                }

                await CreatureCmd.Heal(teammate, DynamicVars.Heal.BaseValue);
            }
        }

        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue + 2);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Heal.UpgradeValueBy(2);
    }
}



