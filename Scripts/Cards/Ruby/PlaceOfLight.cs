using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 每有1点闪耀，该卡的费用减少1。对敌人造成32(48)点伤害。若费用变为0，获得1点能量。

[RegisterCard(typeof(RubyCardPool))]
public class PlaceOfLight : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(32m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public PlaceOfLight() : base(4, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card != this)
        {
            return base.TryModifyEnergyCostInCombat(card, originalCost, out modifiedCost);
        }

        var shine = Owner?.Creature != null ? ShinePowerHelper.GetTotalShine(Owner.Creature) : 0;
        modifiedCost = Math.Max(0m, originalCost - shine);
        return true;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);

        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (EnergyCost.GetWithModifiers(CostModifiers.All) == 0)
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(16);
    }
}



