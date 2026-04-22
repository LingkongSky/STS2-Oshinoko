using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

public class ShellForgedByLiesPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var revenge = RevengePowerHelper.GetTotalRevenge(Owner);
        if (revenge <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var damage = revenge * 6;
        if (CombatHistoryHelper.HasLostHpThisTurn(Owner.Player))
        {
            damage += 8;
        }
        var opponents = combatState.GetOpponentsOf(Owner).ToList();
        await CreatureCmd.Damage(choiceContext, opponents, damage, ValueProp.Move, Owner, null);
    }
}
