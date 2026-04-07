using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Oshinogo.Scripts.Cards.Other;

[Pool(typeof(StatusCardPool))]
public class Stunned : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Exhaust];

    public Stunned() : base(1, CardType.Status, CardRarity.Status, TargetType.None, false)
    {
    }
}
