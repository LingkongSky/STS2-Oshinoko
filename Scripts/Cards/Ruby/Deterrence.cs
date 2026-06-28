using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// 对所有敌人造成5(7)点伤害，给予1层易伤

[RegisterCard(typeof(RubyCardPool))]
public class Deterrence : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("VULNERABLE");
    private const string VulnerableKey = "Vulnerable";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar(VulnerableKey, 1),
    ];

    public Deterrence() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            // No combat state to target opponents.
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var opponents = combatState.GetOpponentsOf(Owner.Creature);
        foreach (var enemy in opponents)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars[VulnerableKey].BaseValue, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}


