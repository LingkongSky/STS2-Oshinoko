using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Oshinoko.Scripts.Patchs;

public static class RestSitePatches
{
    [HarmonyPatch(typeof(Hook), nameof(Hook.ModifyRestSiteOptions))]
    public static class Ownership
    {
        public static bool Prefix(IRunState runState, Player player, ICollection<RestSiteOption> options,
            ref IEnumerable<AbstractModel> __result)
        {
            var applied = new List<AbstractModel>();

            foreach (var model in runState.IterateHookListeners(null))
            {
                if (!CanModifyOptionsForPlayer(model, player))
                {
                    continue;
                }

                if (model.TryModifyRestSiteOptions(player, options))
                {
                    applied.Add(model);
                }
            }

            __result = applied;
            return false;
        }
    }

    private static bool CanModifyOptionsForPlayer(AbstractModel model, Player player)
    {
        return model switch
        {
            CardModel { Owner: { } owner } => ReferenceEquals(owner, player),
            RelicModel { Owner: { } owner } => ReferenceEquals(owner, player),
            PotionModel { Owner: { } owner } => ReferenceEquals(owner, player),
            _ => true
        };
    }
}
