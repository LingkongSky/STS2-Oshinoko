using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 閫犳垚6(8)鐐逛激瀹筹紝濡傛灉杩欏紶鍗＄墝閫犳垚浜?鐐逛激瀹充互涓婏紝鍒欒幏寰?鐐瑰洖鍚堥棯鑰€銆?

[RegisterCard(typeof(RubyCardPool))]
public class InfectiousPassion : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public InfectiousPassion() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);

        var command = await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var dealtDamage = command.Results
            .SelectMany(batch => batch)
            .Where(result => result.Receiver == cardPlay.Target)
            .Sum(result => result.UnblockedDamage + result.OverkillDamage);

        if (dealtDamage >= 9)
        {
            await ShinePowerHelper.ApplyShine(Owner.Creature, 1, ValueDuration.Turn, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}



