using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// ����: ���2����ҫ�����3��غ���ҫ��
public class CapUnderMoon : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    private const string TurnShineKey = "TurnShine";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(2m), new DynamicVar(TurnShineKey, 3m)];

    public CapUnderMoon() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Permanent, Owner.Creature, this);
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[TurnShineKey].BaseValue, ValueDuration.Turn, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


