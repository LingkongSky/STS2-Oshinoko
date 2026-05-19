using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Relics.Aqua;

[RegisterRelic(typeof(AquaRelicPool))]
public class BrotherWatchEX : OshinogoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner?.Creature == null || target != Owner.Creature || result.UnblockedDamage <= 0)
        {
            return;
        }

        Flash();
        await CreatureCmd.Heal(Owner.Creature, 4);
    }
}
