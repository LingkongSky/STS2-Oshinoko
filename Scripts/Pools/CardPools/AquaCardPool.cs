using STS2RitsuLib.Utils;

namespace Oshinogo.Scripts.Pools.CardPools;

public class AquaCardPool : TypeListCardPoolModel
{
    public override string EnergyColorName => "Aqua";
    public override string Title => "Aqua";
    public override string? TextEnergyIconPath => "res://Oshinogo/images/powers/aqua_energy.png";
    public override string? BigEnergyIconPath => "res://Oshinogo/images/powers/aqua_energy_big.png";
    public override Color DeckEntryCardColor => new(0f, 0f, 0f, 1f);
    public override bool IsColorless => false;

    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateUnmodulatedHsvShaderMaterial();
    public override Material? PoolFrameMaterial => _poolFrameMaterial;

}
