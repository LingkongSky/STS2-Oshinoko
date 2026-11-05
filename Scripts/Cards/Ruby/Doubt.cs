using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: �Ե������9(12)���˺��������ɵ��˺�����11(15)����2����ʱ����

[RegisterCard(typeof(RubyCardPool))]
public class Doubt : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    private const string ThresholdKey = "Threshold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar(ThresholdKey, 11),
        new RevengeDynamicVar(2m),
    ];

    public Doubt() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var command = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var dealtDamage = SumDealtDamage(command.Results, cardPlay.Target);
        if (dealtDamage > DynamicVars[ThresholdKey].BaseValue)
        {
            await RevengePowerHelper.ApplyRevenge(
                Owner.Creature,
                DynamicVars[RevengeDynamicVar.Key].BaseValue,
                ValueDuration.Temp,
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars[ThresholdKey].UpgradeValueBy(4);
    }

    private static int SumDealtDamage(IEnumerable<IReadOnlyList<DamageResult>> results, Creature target)
    {
        return results
            .SelectMany(batch => batch)
            .Where(result => result.Receiver == target)
            .Sum(result => result.UnblockedDamage + result.OverkillDamage);
    }
}


