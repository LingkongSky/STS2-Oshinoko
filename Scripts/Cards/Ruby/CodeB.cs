using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 对所有敌人造成6(8)点伤害3次，自己获得6(8)点防御3次。

[Pool(typeof(RubyCardPool))]
public class CodeB : RubyCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        new BlockVar(5m, ValueProp.Move),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public CodeB() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var finalDamage = DynamicVars.CalculatedDamage.Calculate(null);
        var block = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, Owner.Creature);
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            // No combat state to target opponents.
            return;
        }

        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithHitCount(3)
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}
