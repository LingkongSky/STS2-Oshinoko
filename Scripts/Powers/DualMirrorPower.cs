using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

public class DualMirrorPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner != Owner || amount == 0)
        {
            return;
        }

        if (power is not ShinePower and not TurnShinePower and not TempShinePower
            && power is not RevengePower and not TurnRevengePower and not TempRevengePower)
        {
            return;
        }

        if (Owner.Player == null)
        {
            // No player to draw for.
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
    }
}
