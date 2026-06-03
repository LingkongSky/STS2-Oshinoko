namespace Oshinoko.Scripts.Relics.Aqua;

[RegisterRelic(typeof(AquaRelicPool))]
public class BeHatred : OshinokoRelicModel
{
    // 战斗开始时，给予所有人一层陷阱
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeCombatStart()
    {
        var ownerCreature = Owner?.Creature;
        var combatState = ownerCreature?.CombatState;
        if (ownerCreature == null || combatState == null)
        {
            return;
        }

        Flash();
        var allCreatures = combatState.Players
            .Select(player => player.Creature)
            .Concat(combatState.GetOpponentsOf(ownerCreature))
            .Where(creature => creature != null)
            .Distinct()
            .ToList()!;
        foreach (var creature in allCreatures)
        {
            await PowerCmd.Apply<TrapPower>(new BlockingPlayerChoiceContext(), creature, 1, ownerCreature, null, true);
        }
    }
}
