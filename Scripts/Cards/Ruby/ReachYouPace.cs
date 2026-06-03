using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ���14(18)���˺�����ȡ��ͬ������˺��ĸ񵲡�
[RegisterCard(typeof(RubyCardPool))]
public class ReachYouPace : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
    ];

    public ReachYouPace() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var command = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var dealtDamage = SumDealtDamage(command.Results, cardPlay.Target);
        if (dealtDamage > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, dealtDamage, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }

    private static int SumDealtDamage(IEnumerable<IReadOnlyList<DamageResult>> results, Creature target)
    {
        return results
            .SelectMany(batch => batch)
            .Where(result => result.Receiver == target)
            .Sum(result => result.UnblockedDamage + result.OverkillDamage);
    }
}



