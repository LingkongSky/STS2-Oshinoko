using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 每回合开始时获得临时闪耀与临时复仇。
/// </summary>
public class TwoChildsPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await ShinePowerHelper.ApplyShine(Owner, Amount, ValueDuration.Temp, Owner, null);
        await RevengePowerHelper.ApplyRevenge(Owner, Amount, ValueDuration.Temp, Owner, null);
    }
}
