using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.RelicPools;
using Oshinogo.Scripts.RestSite;

namespace Oshinogo.Scripts.Relics.Aqua;

[Pool(typeof(AquaRelicPool))]
// 每次受伤后，回复2点生命。
public class BrotherWatch : OshinogoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<BrotherWatchEX>();

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

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
        {
            return false;
        }

        foreach (RestSiteOption option in options)
        {
            if (option.OptionId == Journey.OptionIdValue)
            {
                return false;
            }
        }

        options.Add(new Journey(player));
        return true;
    }
}
