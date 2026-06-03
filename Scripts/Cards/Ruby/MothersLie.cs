using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ���3��غϸ��������غ���ÿ��ʧȥ�����������е���ʧȥ5��������

[RegisterCard(typeof(RubyCardPool))]
public class MothersLie : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public MothersLie() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await RevengePowerHelper.ApplyRevenge(Owner.Creature, 3, ValueDuration.Turn, Owner.Creature, this);
        await PowerCmd.Apply<MothersLiePower>(choiceContext, Owner.Creature, 5, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



