using STS2RitsuLib.Utils;

namespace Oshinogo.Scripts.Pools.CardPools;

public class RubyCardPool : TypeListCardPoolModel
{
    public override string EnergyColorName => "Ruby";
    public override string Title => "Ruby";
    public override string? TextEnergyIconPath => "res://Oshinogo/images/powers/ruby_energy.png";
    public override string? BigEnergyIconPath => "res://Oshinogo/images/powers/ruby_energy_big.png";
    public override Color DeckEntryCardColor => new(0f, 0f, 0f, 1f);
    public override bool IsColorless => false;

    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateUnmodulatedHsvShaderMaterial();
    public override Material? PoolFrameMaterial => _poolFrameMaterial;
}


