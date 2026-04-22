using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics.Aqua;

[Pool(typeof(AquaRelicPool))]
// 每次受伤后，回复1点生命。
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
        await CreatureCmd.Heal(Owner.Creature, 1);
    }
}
