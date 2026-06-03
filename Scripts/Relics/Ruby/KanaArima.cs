using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Relics.Ruby;
// ÿ��ս����ʼʱ���һ����ҫֵ
[RegisterRelic(typeof(RubyRelicPool))]
public class KanaArima : OshinokoRelicModel
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



