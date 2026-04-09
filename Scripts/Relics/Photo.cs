using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using Oshinogo.Scripts.Pools.RelicPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Relics
{
    // 加入哪个遗物池，此处为通用
    [Pool(typeof(RubyRelicPool))]
    public class Photo : CustomRelicModel
    {
        // 稀有度
        public override RelicRarity Rarity => RelicRarity.Common;


        // 小图标
        public override string PackedIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";
        // 轮廓图标
        protected override string PackedIconOutlinePath => $"res://Oshinogo/images/relics/{GetType().Name}.png";
        // 大图标
        protected override string BigIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";

        public override async Task AfterCombatEnd(CombatRoom room)
        {
            if (Owner?.Creature == null || Owner.Creature.IsDead)
            {
                return;
            }

            var totalShine = ShinePowerHelper.GetTotalShine(Owner.Creature);
            var totalRevenge = RevengePowerHelper.GetTotalRevenge(Owner.Creature);
            var healAmount = 3 + (totalShine * 2) + (totalRevenge * 4);
            if (healAmount <= 0)
            {
                return;
            }

            Flash();
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        }
    }
}
