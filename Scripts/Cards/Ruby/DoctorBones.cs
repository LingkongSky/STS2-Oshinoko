using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: �������ҫȫ��ת��Ϊ���𡣻�õ�����������ʧȥ2��������

[RegisterCard(typeof(RubyCardPool))]
public class DoctorBones : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE", "REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    public DoctorBones() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ShinePowerHelper.GetTotalShine(Owner.Creature);
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            2,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature
        );

        if (total > 0)
        {
            await ShinePowerHelper.LoseShine(Owner.Creature, total, Owner.Creature, this);
            await RevengePowerHelper.ApplyRevenge(Owner.Creature, total, ValueDuration.Permanent, Owner.Creature, this);
            await PlayerCmd.GainEnergy(total, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



