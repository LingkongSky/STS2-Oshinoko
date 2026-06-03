using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 对所有敌人造成7(10)点伤害，敌人在本回合失去2(3)点力量�?
public class DesolateBattlefield : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new DynamicVar("StrengthLoss", 2),
    ];

    public DesolateBattlefield() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        foreach (var enemy in combatState.GetOpponentsOf(Owner.Creature))
        {
            await PowerCmd.Apply<Powers.DesolateBattlefieldPower>(choiceContext, enemy, DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["StrengthLoss"].UpgradeValueBy(1);
    }
}



