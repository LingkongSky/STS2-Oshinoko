using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers;

public abstract class HoshinoAiIconPower : OshinogoCustomPower
{
    public override string? CustomIconPath => "res://Oshinogo/images/powers/ai_energy.png";
    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ai_energy_big.png";
}

// 机制说明：仅用于给玩家查看 Boss 规则。
public class HoshinoAiMechanicsPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}

public class HoshinoAiPhase1DrawGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase1PlayGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase2BlockGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase2EnergyGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiPhase3RebirthGoalPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public class HoshinoAiScrutinyPower : HoshinoAiIconPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (fromHandDraw)
        {
            return true;
        }

        Flash();
        return false;
    }
}

