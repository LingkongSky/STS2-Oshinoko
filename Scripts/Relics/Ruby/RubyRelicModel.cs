using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics.Ruby
{
    [Pool(typeof(RubyRelicPool))]

    public abstract class RubyRelicModel : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Common;

        // 小图标
        public override string PackedIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";
        //public override string PackedIconPath => $"res://Oshinogo/images/relics/Photo.png";

        // 轮廓图标
        protected override string PackedIconOutlinePath => $"res://Oshinogo/images/relics/Photo.png";

        // 大图标
        protected override string BigIconPath => $"res://Oshinogo/images/relics/Photo.png";


    }
}
