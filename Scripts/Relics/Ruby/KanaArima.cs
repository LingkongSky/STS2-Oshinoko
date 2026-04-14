using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using Oshinogo.Scripts.Pools.RelicPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Relics.Ruby;
// 每场战斗开始时获得一点闪耀值
[Pool(typeof(RubyRelicPool))]
public class KanaArima : RubyRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeCombatStart()
    {
        if (Owner?.Creature == null || Owner.Creature.IsDead)
        {
            return;
        }

        Flash();
        await ShinePowerHelper.ApplyShine(Owner.Creature, 1, ValueDuration.Permanent, Owner.Creature, null);
    }
}
