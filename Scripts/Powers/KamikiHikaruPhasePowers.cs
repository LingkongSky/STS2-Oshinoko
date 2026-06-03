using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinoko.Scripts.Powers;

public abstract class KamikiHikaruIconPower : OshinokoCustomPower
{
    public override string? CustomIconPath => "res://Oshinoko/images/powers/ruby_energy_black.png";
    public override string? CustomBigIconPath => "res://Oshinoko/images/powers/ruby_energy_big_black.png";
}

// Shows how much damage players still need to deal in phase 1 to trigger the stun interrupt.
public class KamikiHikaruPhase1StunThresholdPower : KamikiHikaruIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

// Indicates that entering phase 2 will purge all buffs on players and Kamiki.
public class KamikiHikaruPhase2BuffPurgePower : KamikiHikaruIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}

