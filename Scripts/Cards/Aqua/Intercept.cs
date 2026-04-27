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
// 鎻忚堪: 瀵规墍鏈夋晫浜洪€犳垚4(7)鐐逛激瀹筹紝鑾峰緱4(7)鐐规牸鎸°€?
public class Intercept : AquaCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, ValueProp.Move),
        new BlockVar(4, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public Intercept() : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
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
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Block.UpgradeValueBy(3);
    }
}

