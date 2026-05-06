using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Oshinogo.Scripts.Powers;

public class ActCuteNextTurnPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await PowerCmd.Apply<IntangiblePower>(new BlockingPlayerChoiceContext(), Owner, 1, Owner, null, true);
        await PowerCmd.Apply<ActCuteLockoutPower>(new BlockingPlayerChoiceContext(), Owner, Amount, Owner, null, true);
        await PowerCmd.Remove(this);
    }
}

public class ActCuteLockoutPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://Oshinogo/images/powers/ActCuteNextTurnPower.png";
    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ActCuteNextTurnPower.png";

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature != Owner)
        {
            return true;
        }

        return false;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            // Use engine heal signature (creature, amount).
            await CreatureCmd.Heal(Owner, Amount);
            await PowerCmd.Remove(this);
        }
    }
}
