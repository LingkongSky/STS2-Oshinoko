using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinoko.Scripts.Powers;

public abstract class HoshinoAiIconPower : OshinokoCustomPower
{
    public override string? CustomIconPath => "res://Oshinoko/images/powers/ai_energy.png";
    public override string? CustomBigIconPath => "res://Oshinoko/images/powers/ai_energy_big.png";
}

// ����˵���������ڸ���Ҳ鿴 Boss ����
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

