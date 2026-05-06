using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
namespace Oshinogo.Scripts.Powers;

/// <summary>
/// Gain Plan at the start of each turn.
/// </summary>
public class SinkingPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await PowerCmd.Apply<PlanPower>(new BlockingPlayerChoiceContext(), Owner, Amount, Owner, null, true);
    }
}
