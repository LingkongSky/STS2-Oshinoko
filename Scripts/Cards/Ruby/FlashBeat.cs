using STS2RitsuLib.Keywords;

namespace Oshinoko.Scripts.Cards.Ruby;

// 描述: 造成7(10)点伤害。本回合打出过闪耀牌，额外造成7(10)点伤害。

[RegisterCard(typeof(RubyCardPool))]
public class FlashBeat : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public FlashBeat() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        var hadShine = ShinePowerHelper.GetTotalShine(Owner.Creature) > 0;
        var playedShineEarlier = CombatHistoryHelper.HasPlayedShineCardWithShineThisTurn(Owner);

        if (hadShine && playedShineEarlier)
        {
            finalDamage += finalDamage;
        }

        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}




