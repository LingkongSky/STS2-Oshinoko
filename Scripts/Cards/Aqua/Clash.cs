using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 造成10(15)点伤害，如果敌人的意图是攻击，再造成15(20)点伤害。
public class Clash : AquaCardModel
{
    private const string BonusDamageKey = "BonusDamage";
    private const string CalculatedBonusDamageKey = "CalculatedBonusDamage";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new DynamicVar(BonusDamageKey, 15),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        CreateCalculatedBonusDamageVar(),
    ];

    public Clash() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (cardPlay.Target.Monster?.IntendsToAttack == true)
        {
            var bonusDamage = CalculateBonusDamage();
            await DamageCmd.Attack(bonusDamage)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }
    }

    private decimal CalculateBonusDamage()
    {
        var baseDamage = DynamicVars[BonusDamageKey].BaseValue;
        var extraPerShine = DynamicVars.CalculationExtra.BaseValue;
        var shine = ShinePowerHelper.GetTotalShine(Owner.Creature);
        var revenge = RevengePowerHelper.GetTotalRevenge(Owner.Creature);
        var revengeMultiplier = revenge > 0 ? revenge : 1;
        return (baseDamage + extraPerShine * shine) * revengeMultiplier;
    }

    private static CalculatedVar CreateCalculatedBonusDamageVar()
    {
        return new ClashCalculatedBonusDamageVar(CalculatedBonusDamageKey)
            .WithMultiplier((card, _) =>
            {
                var baseValue = card.DynamicVars[BonusDamageKey].BaseValue;
                var extra = card.DynamicVars.CalculationExtra.BaseValue;
                var shine = ShinePowerHelper.GetTotalShine(card.Owner.Creature);
                var revenge = RevengePowerHelper.GetTotalRevenge(card.Owner.Creature);
                var revengeMultiplier = revenge > 0 ? revenge : 1;
                if (extra == 0)
                {
                    return revengeMultiplier - 1;
                }

                var finalValue = (baseValue + extra * shine) * revengeMultiplier;
                return (finalValue - baseValue) / extra;
            });
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars[BonusDamageKey].UpgradeValueBy(5);
    }
}

public class ClashCalculatedBonusDamageVar(string name) : CalculatedVar(name)
{
    protected override DynamicVar GetBaseVar()
    {
        return ((CardModel)_owner!).DynamicVars["BonusDamage"];
    }
}




