using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Oshinoko.Scripts.UI;

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class OshinokoCreatureDeathAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (!ShouldApplyToOshinokoPlayer(__instance))
        {
            return;
        }

        if (__instance.Visuals is OshinokoNCreatureVisuals visuals)
        {
            visuals.PlayDeathFallAnim();
        }
    }

    private static bool ShouldApplyToOshinokoPlayer(NCreature nCreature)
    {
        Player? player = nCreature.Entity?.Player;
        if (player?.Character == null)
        {
            return false;
        }

        string typeName = player.Character.GetType().Name;
        return typeName == "Aqua" || typeName == "Ruby";
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartReviveAnim))]
public static class OshinokoCreatureReviveAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals is OshinokoNCreatureVisuals visuals)
        {
            visuals.ResetDeathFallState();
        }
    }
}
