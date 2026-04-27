using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 在拥有者下回合开始时，对所有敌人造成伤害并移除。
/// </summary>
public class FarawayPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            await PowerCmd.Remove(this);
            return;
        }

        var opponents = combatState.GetOpponentsOf(Owner).ToList();
        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), opponents, Amount, ValueProp.Move, Owner, null);

        await PowerCmd.Remove(this);
    }
}
