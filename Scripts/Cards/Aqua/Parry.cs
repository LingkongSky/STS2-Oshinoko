using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 造成13(17)点伤害，获得13(17)点格挡。 谋划1
public class Parry : AquaCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";
    protected override IEnumerable<IHoverTip> ExtraHoverTips => PlanCostHelper.CreatePlanCostHoverTips(1);

    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13, ValueProp.Move),
        new BlockVar(13, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public Parry() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 1))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
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
