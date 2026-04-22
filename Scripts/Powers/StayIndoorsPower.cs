using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

public class StayIndoorsPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _triggerNextTurn;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
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

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
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
