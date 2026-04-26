using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 下一个敌方回合开始时（即玩家下回合开始时），目标受到伤害并移除。
/// </summary>
public class FarawayPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player.Creature == null || player.Creature.Side == Owner.Side)
        {
            return;
        }

        await CreatureCmd.Damage(
            new BlockingPlayerChoiceContext(),
            Owner,
            Amount,
            ValueProp.Move,
            player.Creature,
            null
        );

        await PowerCmd.Remove(this);
    }
}

