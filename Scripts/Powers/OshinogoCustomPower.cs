using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers
{
    public abstract class OshinogoCustomPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override string? CustomPackedIconPath => ResolveIconPath();
        public override string? CustomBigIconPath => ResolveIconPath();

        private string ResolveIconPath()
        {
            var candidate = $"res://Oshinogo/images/powers/{GetType().Name}.png";
            if (ResourceLoader.Exists(candidate))
            {
                return candidate;
            }

            return "res://Oshinogo/images/relics/Photo.png";
        }
    }
}
