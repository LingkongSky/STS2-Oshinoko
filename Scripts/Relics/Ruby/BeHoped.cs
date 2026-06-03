namespace Oshinoko.Scripts.Relics.Ruby;

[RegisterRelic(typeof(RubyRelicPool))]
public class BeHoped : OshinokoRelicModel
{
    // 战斗开始时，给予自己一层无实体
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeCombatStart()
    {
        var ownerCreature = Owner?.Creature;
        if (ownerCreature == null)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<IntangiblePower>(new BlockingPlayerChoiceContext(), ownerCreature, 1, ownerCreature, null, true);
    }
}
