using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

public abstract class AquaCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://Oshinogo/images/cards/aqua/{GetType().Name}.png";


    public AquaCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
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
