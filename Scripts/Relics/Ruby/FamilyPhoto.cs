using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Relics.Ruby
{
    [RegisterRelic(typeof(RubyRelicPool))]
    public class FamilyPhoto : OshinogoRelicModel
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
            var healAmount = 5 + (totalShine * 3) + (totalRevenge * 4);
            if (healAmount <= 0)
            {
                return;
            }

            Flash();
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        }
    }
}
