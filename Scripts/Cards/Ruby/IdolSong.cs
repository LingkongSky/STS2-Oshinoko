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

// 描述: 消耗所有闪耀值，对所有敌人造成2点伤害X次，X=5+消耗闪耀值的一半(向下取整)
[Pool(typeof(RubyCardPool))]
public class IdolSong : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public IdolSong() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var totalShine = ShinePowerHelper.GetTotalShine(Owner.Creature);
        var hits = 4 + totalShine / 2;

        if (totalShine > 0)
        {
            await ShinePowerHelper.LoseShine(Owner.Creature, totalShine, Owner.Creature, this);
        }

        if (hits <= 0)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitCount(hits)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
