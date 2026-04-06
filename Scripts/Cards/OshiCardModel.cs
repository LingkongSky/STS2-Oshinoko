using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;


public abstract class OshiCardModel : CustomCardModel
{
    //public override string PortraitPath => $"res://Oshinogo/images/cards/{GetType().Name}.png";
    public override string PortraitPath => $"res://Oshinogo/images/cards/Strike.png";

    public OshiCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

}


