using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 所有队友获得1点能量并抽1张牌。

[RegisterCard(typeof(RubyCardPool))]
public class SpotlightShare : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine.GetModKeywordCardKeyword()];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public SpotlightShare() : base(2, CardType.Skill, CardRarity.Event, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var teammates = combatState.GetTeammatesOf(Owner.Creature);
        foreach (var teammate in teammates)
        {
            teammate.Player?.PlayerCombatState?.GainEnergy(1);
            if (teammate.Player != null)
            {
                await CardPileCmd.Draw(choiceContext, 1, teammate.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



