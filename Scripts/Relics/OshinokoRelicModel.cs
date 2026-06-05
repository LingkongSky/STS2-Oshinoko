
namespace Oshinoko.Scripts.Relics
{

    public abstract class OshinokoRelicModel : ModRelicTemplate
    {
        public override RelicRarity Rarity => RelicRarity.Common;

        // 小图标
        public override string PackedIconPath => $"res://Oshinoko/images/relics/{GetType().Name}.png";

        // 轮廓图标
        protected override string PackedIconOutlinePath => $"res://Oshinoko/images/relics/{GetType().Name}.png";

        // 大图标
        protected override string BigIconPath => $"res://Oshinoko/images/relics/{GetType().Name}.png";


    }
}

