using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers;

public class ShinePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/ui/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";
}
