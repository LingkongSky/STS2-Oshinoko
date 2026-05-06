using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Oshinogo.Scripts.Relics
{

    public abstract class OshinogoRelicModel : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Common;

        // 小图标
        public override string PackedIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";
        //public override string PackedIconPath => $"res://Oshinogo/images/relics/Photo.png";

        // 轮廓图标
        protected override string PackedIconOutlinePath => $"res://Oshinogo/images/relics/{GetType().Name}.png";

        // 大图标
        protected override string BigIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";


    }
}
