using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics.Aqua;

[Pool(typeof(AquaRelicPool))]
// 描述: 每三个回合翻倍一次自身拥有的正面效果层数。
public class Akane : OshinogoRelicModel
{
    private const int CycleLength = 3;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool ShowCounter => DisplayAmount > -1;

    public override int DisplayAmount
    {
        get
        {
            if (!CombatManager.Instance.IsInProgress || IsCanonical || Owner?.Creature?.CombatState == null)
            {
                return -1;
            }

            var roundNumber = Owner.Creature.CombatState.RoundNumber;
            if (roundNumber <= 0)
            {
                return 1;
            }

            return ((roundNumber - 1) % CycleLength) + 1;
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner?.Creature != null && side == Owner.Creature.Side)
        {
            Status = combatState.RoundNumber % CycleLength == 0 ? RelicStatus.Active : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (Owner?.Creature?.CombatState == null || player != Owner)
        {
            return;
        }

        var roundNumber = Owner.Creature.CombatState.RoundNumber;
        if (roundNumber <= 0 || roundNumber % CycleLength != 0)
        {
            Status = RelicStatus.Normal;
            InvokeDisplayAmountChanged();
            return;
        }

        var buffs = Owner.Creature.Powers
            .Where(power => power.Type == PowerType.Buff
                && power.StackType == PowerStackType.Counter
                && power.Amount > 0)
            .ToList();
        if (buffs.Count == 0)
        {
            InvokeDisplayAmountChanged();
            return;
        }

        Flash();
        foreach (var buff in buffs)
        {
            await PowerCmd.ModifyAmount(buff, buff.Amount, Owner.Creature, null);
        }

        InvokeDisplayAmountChanged();
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Status = RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }
}
