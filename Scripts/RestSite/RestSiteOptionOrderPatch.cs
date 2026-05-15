using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace Oshinogo.Scripts.RestSite;

[HarmonyPatch(typeof(RestSiteOption), nameof(RestSiteOption.Generate))]
public static class RestSiteOptionOrderPatch
{
    private static readonly string[] BaseOrder = { "HEAL", "SMITH", "MEND" };

    public static void Postfix(Player player, ref List<RestSiteOption> __result)
    {
        if (__result == null || __result.Count <= 1)
        {
            return;
        }

        var byId = __result
            .Where(option => option != null)
            .ToDictionary(option => option.OptionId, option => option);

        var ordered = new List<RestSiteOption>(__result.Count);

        foreach (string id in BaseOrder)
        {
            if (byId.TryGetValue(id, out RestSiteOption? option) && option != null)
            {
                ordered.Add(option);
            }
        }

        foreach (RestSiteOption option in __result)
        {
            if (option == null)
            {
                continue;
            }

            if (Array.IndexOf(BaseOrder, option.OptionId) >= 0)
            {
                continue;
            }

            ordered.Add(option);
        }

        ordered = ordered
            .Take(BaseOrder.Length)
            .Concat(ordered.Skip(BaseOrder.Length)
            .OrderBy(o => o.OptionId, StringComparer.Ordinal))
            .ToList();

        __result = ordered;
    }
}
