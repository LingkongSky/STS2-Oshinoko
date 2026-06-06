using MegaCrit.Sts2.Core.Entities.RestSite;
using Oshinoko.Scripts.RestSite;

namespace Oshinoko.Scripts.Relics.Ruby
{
    [RegisterRelic(typeof(RubyRelicPool))]
    [RegisterCharacterStarterRelic(typeof(Character.Ruby))]
    public class Photo : OshinokoRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;

        public override bool TryModifyRestSiteOptions(Player owner, ICollection<RestSiteOption> options)
        {
            if (!ReferenceEquals(Owner, owner)
                || Owner?.Character is not Oshinoko.Scripts.Character.Ruby
                || owner.Character is not Oshinoko.Scripts.Character.Ruby)
            {
                return false;
            }

            if (options.Any(option => option.OptionId == BKomachiGathering.OptionIdValue))
            {
                return false;
            }

            options.Add(new BKomachiGathering(owner));
            return true;
        }

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
    }
}
