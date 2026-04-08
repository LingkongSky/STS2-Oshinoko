using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 每有1点闪耀值，该卡的费用减少1，对敌人造成30(45)点伤害
[Pool(typeof(RubyCardPool))]
public class PlaceOfLight : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(27m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public PlaceOfLight() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
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

        var shine = ShinePowerHelper.GetTotalShine(Owner.Creature);
        var refund = Math.Min(shine, EnergyCost.GetWithModifiers(CostModifiers.All));
        if (refund > 0)
        {
            await PlayerCmd.GainEnergy(refund, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(13);
    }
}
