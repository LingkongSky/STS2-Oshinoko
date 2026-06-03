using STS2RitsuLib.Scaffolding.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinoko.Scripts.Powers
{
    public abstract class OshinokoCustomPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override string? CustomIconPath => ResolveIconPath();
        public override string? CustomBigIconPath => ResolveIconPath();

        private string ResolveIconPath()
        {
            var candidate = $"res://Oshinoko/images/powers/{GetType().Name}.png";
            if (ResourceLoader.Exists(candidate))
            {
                return candidate;
            }

            return "res://Oshinoko/images/relics/Photo.png";
        }
    }
}

