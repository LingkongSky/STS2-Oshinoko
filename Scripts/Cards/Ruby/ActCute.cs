using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: �»غϻ����ʵ�壬�Ҳ��ܳ��ơ��»غϽ�����ظ�5(7)��������

[RegisterCard(typeof(RubyCardPool))]
public class ActCute : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(5m),
    ];

    public ActCute() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ActCuteNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Heal.BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2);
        EnergyCost.UpgradeBy(-1);
    }
}



