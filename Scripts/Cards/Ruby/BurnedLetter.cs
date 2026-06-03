using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ȥ�����ж��ѵĸ���Ч�����»غ��������ж��ѻ��1(2)��������

[RegisterCard(typeof(RubyCardPool))]
public class BurnedLetter : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public BurnedLetter() : base(1, CardType.Skill, CardRarity.Event, TargetType.Self, true)
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
            var debuffs = teammate.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
            foreach (var debuff in debuffs)
            {
                await PowerCmd.Remove(debuff);
            }

            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, teammate, DynamicVars.Energy.BaseValue, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}



