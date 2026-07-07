using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 给予所有敌人5层流言，并造成10(14)点伤害。 谋划1
public class Bond : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => PlanAndKeywordTips(1, "RUMOR");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new DynamicVar("Rumor", 5),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public Bond() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 1))
        {
            return;
        }

        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        foreach (var enemy in combatState.GetOpponentsOf(Owner.Creature))
        {
            await PowerCmd.Apply<RumorPower>(choiceContext, enemy, DynamicVars["Rumor"].BaseValue, Owner.Creature, this, true);
        }

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}



