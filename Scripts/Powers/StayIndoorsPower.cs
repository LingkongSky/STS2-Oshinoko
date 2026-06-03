using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinoko.Scripts.Cards.Other;

namespace Oshinoko.Scripts.Powers;

public class StayIndoorsPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _triggerNextTurn;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        if (!CombatHistoryHelper.HasLostHpThisTurn(Owner.Player))
        {
            return;
        }

        _triggerNextTurn = true;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return;
        }

        if (!_triggerNextTurn)
        {
            return;
        }

        _triggerNextTurn = false;
        await RevengePowerHelper.ApplyRevenge(Owner, 2, ValueDuration.Temp, Owner, null);
        await CreatureCmd.GainBlock(Owner, 5, ValueProp.Move, null);
    }
}

