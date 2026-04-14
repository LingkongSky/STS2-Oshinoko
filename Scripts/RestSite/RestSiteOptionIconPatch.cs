using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace Oshinogo.Scripts.RestSite;

[HarmonyPatch(typeof(RestSiteOption), "Icon", MethodType.Getter)]
public static class RestSiteOptionIconPatch
{
    public static bool Prefix(RestSiteOption __instance, ref Godot.Texture2D __result)
    {
        if (__instance.OptionId != BKomachiGathering.OptionIdValue)
        {
            return true;
        }

        __result = PreloadManager.Cache.GetTexture2D(BKomachiGathering.IconPath);
        return false;
    }
}
