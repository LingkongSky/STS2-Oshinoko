using STS2RitsuLib.Utils;

namespace Oshinoko.Scripts.Pools.CardPools;

public class RubyCardPool : TypeListCardPoolModel
{
    public override string EnergyColorName => "Ruby";
    public override string Title => "Ruby";
    public override string? TextEnergyIconPath => "res://Oshinoko/images/powers/ruby_energy.png";
    public override string? BigEnergyIconPath => "res://Oshinoko/images/powers/ruby_energy_big.png";
    public override Color DeckEntryCardColor => new(1f, 0.4f, 0.8f);
    public override Color EnergyOutlineColor => new(1f, 0.4f, 0.8f);
    public override bool IsColorless => false;

    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateUnmodulatedHsvShaderMaterial();
    public override Material? PoolFrameMaterial => _poolFrameMaterial;
}


