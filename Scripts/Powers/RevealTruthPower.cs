using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinoko.Scripts.Powers;

public class RevealTruthPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        if (Owner.MaxHp <= 0)
        {
            return;
        }

        // Use current HP for half-health check.
        if (Owner.CurrentHp < Owner.MaxHp / 2m)
        {
            await RevengePowerHelper.ApplyRevenge(Owner, 1, ValueDuration.Permanent, Owner, null);
        }
    }
}
