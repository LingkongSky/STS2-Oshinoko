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
    public override string? CustomIconPath => "res://Oshinogo/images/powers/ActCuteNextTurnPower.png";
    public override string? CustomBigIconPath => "res://Oshinogo/images/powers/ActCuteNextTurnPower.png";

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature != Owner)
        {
            return true;
        }

        return false;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            // Use engine heal signature (creature, amount).
            await CreatureCmd.Heal(Owner, Amount);
            await PowerCmd.Remove(this);
        }
    }
}


