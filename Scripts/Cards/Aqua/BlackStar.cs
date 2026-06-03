using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// ����: ÿ�λ��ı��ʱ������һ��ı����
public class BlackStar : AquaCardModel
{
    private bool _isInnateWhenUpgraded;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        _isInnateWhenUpgraded ? [CardKeyword.Innate] : [];

    public BlackStar() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BlackStarPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        _isInnateWhenUpgraded = true;
        EnergyCost.UpgradeBy(-1);
    }
}



