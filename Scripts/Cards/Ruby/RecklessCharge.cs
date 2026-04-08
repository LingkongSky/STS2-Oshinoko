using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 造成4(6)点伤害2次，消耗1张牌，并获取等同于该卡费用的临时闪耀值
[Pool(typeof(RubyCardPool))]
public class RecklessCharge : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public RecklessCharge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);

        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(2)
            .Execute(choiceContext);

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, _ => true, this)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        var cost = selected.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (cost < 0)
        {
            cost = 0;
        }

        await CardCmd.Exhaust(choiceContext, selected);
        await ShinePowerHelper.ApplyShine(Owner.Creature, cost, ValueDuration.Temp, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
