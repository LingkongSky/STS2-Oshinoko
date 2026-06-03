using System;
using System.Collections.Generic;
using STS2RitsuLib.Scaffolding.Content;

namespace Oshinoko.Scripts.Pools.PotionPools;

public class RubyPotionPool : TypeListPotionPoolModel
{
    public override string EnergyColorName => "Ruby";
    public override string? TextEnergyIconPath => "res://Oshinoko/images/powers/ruby_energy.png";
    public override string? BigEnergyIconPath => "res://Oshinoko/images/powers/ruby_energy_big.png";

    [Obsolete]
    protected override IEnumerable<Type> PotionTypes => Array.Empty<Type>();
}


