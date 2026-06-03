using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// ����: ����������ӵ�е�����Ч���Ĳ�����
public class SideBySide : AquaCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public SideBySide() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var buffs = Owner.Creature.Powers
            .Where(power => power.Type == PowerType.Buff
                && power.StackType == PowerStackType.Counter
                && power.Amount > 0)
            .ToList();
        foreach (var buff in buffs)
        {
            await PowerCmd.ModifyAmount(choiceContext, buff, buff.Amount, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}




