namespace Oshinoko.Scripts.Cards.Other;

[RegisterCard(typeof(RubyCardPool))]
public class Stunned : RubyCardModel
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Unplayable];


    public Stunned() : base(-1, CardType.Status, CardRarity.Status, TargetType.None, false)
    {
    }
}

