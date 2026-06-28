using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 
public class Hatred : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => PlanCostHelper.CreatePlanCostHoverTips(1);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];

    public Hatred() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var debuffs = cardPlay.Target.Powers.Where(power => power.Type == PowerType.Debuff && power.Amount > 0).ToList();
        foreach (var debuff in debuffs)
        {
            await PowerCmd.ModifyAmount(choiceContext, debuff, debuff.Amount, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


