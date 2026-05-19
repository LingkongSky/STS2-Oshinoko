using STS2RitsuLib.Scaffolding.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers
{
    public abstract class OshinogoCustomPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override string? CustomIconPath => ResolveIconPath();
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

