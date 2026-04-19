using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Rooms;
using Oshinogo.Scripts.Pools.RelicPools;
using Oshinogo.Scripts.Powers;
using Oshinogo.Scripts.RestSite;

// 每场战斗结束时，为露比回复5+3x闪耀+5x复仇的血量。
namespace Oshinogo.Scripts.Relics.Ruby
{
    [Pool(typeof(RubyRelicPool))]
    public class FamilyPhoto : RubyRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Ancient;

        public override async Task AfterCombatEnd(CombatRoom room)
        {
            if (Owner?.Creature == null || Owner.Creature.IsDead)
            {
                return;
            }

            var totalShine = ShinePowerHelper.GetTotalShine(Owner.Creature);
            var totalRevenge = RevengePowerHelper.GetTotalRevenge(Owner.Creature);
            var healAmount = 5 + (totalShine * 3) + (totalRevenge * 5);
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
