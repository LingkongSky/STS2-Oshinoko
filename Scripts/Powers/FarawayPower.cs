using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinoko.Scripts.Powers;

/// <summary>
/// ��ӵ�����»غϿ�ʼʱ�������е�������˺����Ƴ���
/// </summary>
public class FarawayPower : OshinokoCustomPower
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
