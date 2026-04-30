using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Oshinogo.Scripts.UI;

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class OshinogoCreatureDeathAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (!ShouldApplyToOshinogoPlayer(__instance))
        {
            return;
        }

        if (__instance.Visuals is OshinogoNCreatureVisuals visuals)
        {
            visuals.PlayDeathFallAnim();
        }
    }

    private static bool ShouldApplyToOshinogoPlayer(NCreature nCreature)
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
public static class OshinogoCreatureReviveAnimPatch
{
    private static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals is OshinogoNCreatureVisuals visuals)
        {
            visuals.ResetDeathFallState();
        }
    }
}
