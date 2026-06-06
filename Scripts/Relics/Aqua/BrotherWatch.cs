using MegaCrit.Sts2.Core.Entities.RestSite;
using Oshinoko.Scripts.RestSite;

namespace Oshinoko.Scripts.Relics.Aqua;

[RegisterRelic(typeof(AquaRelicPool))]
[RegisterCharacterStarterRelic(typeof(Character.Aqua))]

public class BrotherWatch : OshinokoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool TryModifyRestSiteOptions(Player owner, ICollection<RestSiteOption> options)
    {
        if (!ReferenceEquals(Owner, owner)
            || Owner?.Character is not Oshinoko.Scripts.Character.Aqua
            || owner.Character is not Oshinoko.Scripts.Character.Aqua)
        {
            return false;
        }

        if (options.Any(option => option.OptionId == Journey.OptionIdValue))
        {
            return false;
        }

        options.Add(new Journey(owner));
        return true;
    }

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
        await CreatureCmd.Heal(Owner.Creature, 2);
    }
}
