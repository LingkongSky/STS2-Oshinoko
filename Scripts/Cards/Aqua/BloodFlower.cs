namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 被消耗时获得一层谋划。
public class BloodFlower : AquaCardModel
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    public BloodFlower() : base(-1, CardType.Status, CardRarity.Status, TargetType.None, false)
    {
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (!ReferenceEquals(card, this))
        {
            return;
        }

        await PowerCmd.Apply<PlanPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }
}



