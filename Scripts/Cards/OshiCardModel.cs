using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

public abstract class OshiCardModel : CustomCardModel
{
    public override string PortraitPath => $"res://Oshinogo/images/cards/{GetType().Name}.png";
    //public override string PortraitPath => $"res://Oshinogo/images/cards/Strike.png";


    
    public OshiCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }


    public override Material? CreateCustomFrameMaterial
    {
        get
        {
            // 创建一个默认 ShaderMaterial，但不改变任何颜色
            Shader shader = new Shader();
            shader.Code = @"
            shader_type canvas_item;

            void fragment() {
                // 直接输出纹理本身颜色，不做任何修改
                COLOR = texture(TEXTURE, UV);
            }
        ";

            ShaderMaterial mat = new ShaderMaterial();
            mat.Shader = shader;

            return mat;
        }
    }

}
