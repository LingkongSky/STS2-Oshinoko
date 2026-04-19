using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace Oshinogo.Scripts.Pools.CardPools
{
    public class RubyCardPool : CustomCardPoolModel
    {
        // 卡池的ID。必须唯一防撞车。
        public override string Title => "Ruby";

        // 描述中使用的能量图标。大小为24x24。
        public override string? TextEnergyIconPath => "res://Oshinogo/images/ui/ruby_energy.png";
        // tooltip和卡牌左上角的能量图标。大小为74x74。
        public override string? BigEnergyIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";

        // 卡池的主题色。
        public override Color DeckEntryCardColor => new(0f, 0f, 0f, 1f);

        // 如果你使用默认的卡框，可以使用这个颜色来修改卡框的颜色。
        // public override Color ShaderColor => new(1f, 0.4f, 0.8f);

        

        // 如果你使用自定义卡框图片，重写CustomFrame方法并返回你的卡框图片。
        public override Texture2D? CustomFrame(CustomCardModel card)
        {
            return card.Type switch
            {
                CardType.Attack => PreloadManager.Cache.GetAsset<Texture2D>("res://Oshinogo/images/ui/card_frame/ruby_attack.png"),
                CardType.Power => PreloadManager.Cache.GetAsset<Texture2D>("res://Oshinogo/images/ui/card_frame/ruby_power.png"),
                _ => PreloadManager.Cache.GetAsset<Texture2D>("res://Oshinogo/images/ui/card_frame/ruby_skill.png"),
            };
        }

        // 卡池是否是无色。例如事件、状态等卡池就是无色的。
        public override bool IsColorless => false;
    }
}
