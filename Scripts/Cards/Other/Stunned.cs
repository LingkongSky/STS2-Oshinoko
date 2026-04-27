using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Other;

[Pool(typeof(StatusCardPool))]
public class Stunned : RubyCardModel
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [        CardKeyword.Ethereal,
        CardKeyword.Unplayable];


    public Stunned() : base(-1, CardType.Status, CardRarity.Status, TargetType.None, false)
    {
    }
}
