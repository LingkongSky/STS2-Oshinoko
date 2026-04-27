using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 对所有敌人造成12(16)点伤害，获得12(16)点格挡。谋划1
public class IceAndFire : AquaCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => PlanCostHelper.CreatePlanCostHoverTips(1);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new BlockVar(12, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public IceAndFire() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, true)
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

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var block = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars.Block.UpgradeValueBy(4);
    }
}
