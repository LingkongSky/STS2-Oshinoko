using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers
{
    public abstract class OshinogoCustomPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        // 叠加类型，Counter表示可叠加，Single表示不可叠加
        public override PowerStackType StackType => PowerStackType.Counter;

        public override string? CustomPackedIconPath => "res://Oshinogo/images/relics/Photo.png";
        public override string? CustomBigIconPath => "res://Oshinogo/images/relics/Photo.png";
    }
}
