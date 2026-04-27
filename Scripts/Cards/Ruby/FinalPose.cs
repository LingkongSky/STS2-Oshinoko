using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 浠呭綋闂€€鍊煎ぇ浜?鏃舵墠鑳芥墦鍑恒€傞€犳垚24(34)鐐逛激瀹筹紝缁欎簣2灞傛槗浼ゃ€?

[Pool(typeof(RubyCardPool))]
public class FinalPose : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("VULNERABLE", "SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, OshinogoKeywords.Shine];

    private const int RequiredShine = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(24m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        ];

    public FinalPose() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override bool IsPlayable => Owner?.Creature != null && ShinePowerHelper.GetTotalShine(Owner.Creature) > RequiredShine;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, 2, Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}
