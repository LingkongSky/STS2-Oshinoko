using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Cards;
using Oshinogo.Scripts.Cards.Other;

public abstract class RubyCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://Oshinogo/images/cards/ruby/{GetType().Name}.png";


    public RubyCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected static IEnumerable<IHoverTip> KeywordTips(params string[] keys)
    {
        return CardKeywordHoverTipHelper.Create(keys);
    }

    protected static IEnumerable<IHoverTip> MergeKeywordTips(IEnumerable<IHoverTip> primary, params string[] keys)
    {
        return CardKeywordHoverTipHelper.Merge(primary, CardKeywordHoverTipHelper.Create(keys));
    }


    public override Material? CreateCustomFrameMaterial
    {
        get
        {
            Shader shader = new Shader();
            shader.Code = @"
            shader_type canvas_item;

            void fragment() {
                COLOR = texture(TEXTURE, UV);
            }
        ";

            ShaderMaterial mat = new ShaderMaterial();
            mat.Shader = shader;

            return mat;
        }
    }

}
