using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

public abstract class OshiCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://Oshinogo/images/cards/{GetType().Name}.png";
    //public override string PortraitPath => $"res://Oshinogo/images/cards/Strike.png";

    private static readonly Dictionary<CardType, Color> _typeFrameColors = new()
    {
        { CardType.Attack, new Color(1f, 0.4f, 0.8f) },
        { CardType.Skill, new Color(1f, 0.4f, 0.8f) },
        { CardType.Power, new Color(1f, 0.4f, 0.8f) },
        { CardType.Quest, new Color(1f, 0.4f, 0.8f) },
    };

    public OshiCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    public override Material? CreateCustomFrameMaterial
    {
        get
        {
            Color color = ResolveFrameColorByType();
            return ShaderUtils.GenerateHsv(color.H, color.S, color.V);
        }
    }

    protected virtual Color ResolveFrameColorByType()
    {
        if (_typeFrameColors.TryGetValue(Type, out var color))
        {
            return color;
        }
        if (Pool is CustomCardPoolModel customPool)
        {
            return customPool.ShaderColor;
        }
        return new Color(1f, 0.3f, 0.4f);
    }

}
