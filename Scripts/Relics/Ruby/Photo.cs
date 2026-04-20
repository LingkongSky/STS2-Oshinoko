using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Oshinogo.Scripts.Pools.RelicPools;
using Oshinogo.Scripts.Powers;
using Oshinogo.Scripts.RestSite;

// 每场战斗结束时，为露比回复3+1x闪耀+3x复仇的血量。
namespace Oshinogo.Scripts.Relics.Ruby
{
    [Pool(typeof(RubyRelicPool))]
    public class Photo : RubyRelicModel
    {
        // 稀有度
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<FamilyPhoto>();
        public override async Task AfterCombatEnd(CombatRoom room)
        {
            if (Owner?.Creature == null || Owner.Creature.IsDead)
            {
                return;
            }

            var totalShine = ShinePowerHelper.GetTotalShine(Owner.Creature);
            var totalRevenge = RevengePowerHelper.GetTotalRevenge(Owner.Creature);
            var healAmount = 3 + (totalShine * 1) + (totalRevenge * 3);
            if (healAmount <= 0)
            {
                return;
            }

            Flash();
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        }

        public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
        {
            if (player != Owner)
            {
                return false;
            }

            foreach (RestSiteOption option in options)
            {
                if (option.OptionId == BKomachiGathering.OptionIdValue)
                {
                    return false;
                }
            }

            options.Add(new BKomachiGathering(player));
            return true;
        }
    }
}
