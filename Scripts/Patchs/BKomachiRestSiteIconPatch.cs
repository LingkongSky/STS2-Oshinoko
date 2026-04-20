using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using Oshinogo.Scripts.RestSite;

namespace Oshinogo.Scripts.Patchs;

// 篝火选项Patch
[HarmonyPatch(typeof(NRestSiteButton), "Reload")]
public static class BKomachiRestSiteIconPatch
{
    private static readonly AccessTools.FieldRef<NRestSiteButton, TextureRect> IconField =
        AccessTools.FieldRefAccess<NRestSiteButton, TextureRect>("_icon");

    private static readonly AccessTools.FieldRef<NRestSiteButton, GodotObject> HsvField =
        AccessTools.FieldRefAccess<NRestSiteButton, GodotObject>("_hsv");

    private static readonly AccessTools.FieldRef<NRestSiteButton, MegaCrit.Sts2.addons.mega_text.MegaLabel> LabelField =
        AccessTools.FieldRefAccess<NRestSiteButton, MegaCrit.Sts2.addons.mega_text.MegaLabel>("_label");

    public static bool Prefix(NRestSiteButton __instance)
    {
        if (__instance.Option is not BKomachiGathering)
        {
            return true;
        }

        TextureRect icon = IconField(__instance);
        if (icon != null)
        {
            icon.Texture = PreloadManager.Cache.GetTexture2D(BKomachiGathering.IconPath);
        }

        var label = LabelField(__instance);
        label?.SetTextAutoSize(__instance.Option.Title.GetFormattedText());

        if (HsvField(__instance) is ShaderMaterial hsv)
        {
            if (!__instance.Option.IsEnabled)
            {
                hsv.SetShaderParameter("s", 0f);
                hsv.SetShaderParameter("v", 0.6f);
            }
            else
            {
                hsv.SetShaderParameter("s", 1f);
                hsv.SetShaderParameter("v", 1f);
            }
        }

        return false;
    }
}
