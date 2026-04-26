using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 对敌人造成15(20)点伤害，给予1(2)层陷阱。谋划2
public class BiteTight : AquaCardModel
{
    protected override IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> ExtraHoverTips => PlanCostHelper.CreatePlanCostHoverTips(2);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15, ValueProp.Move),
        new DynamicVar("Trap", 1),
    ];

    public BiteTight() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 2);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 2))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<TrapPower>(cardPlay.Target, DynamicVars["Trap"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars["Trap"].UpgradeValueBy(1);
    }
}
