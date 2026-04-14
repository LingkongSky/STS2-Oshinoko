using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace Oshinogo.Scripts.UI;

[HarmonyPatch(typeof(NPotionHolder), nameof(NPotionHolder.ShineOnStartOfCombat))]
public static class NPotionHolderShineSafetyPatch
{
    public static bool Prefix(NPotionHolder __instance, ref Task __result)
    {
        if (!GodotObject.IsInstanceValid(__instance) || __instance.IsQueuedForDeletion())
        {
            __result = Task.CompletedTask;
            return false;
        }

        var potion = __instance.Potion;
        if (potion == null || !GodotObject.IsInstanceValid(potion) || potion.IsQueuedForDeletion())
        {
            __result = Task.CompletedTask;
            return false;
        }

        return true;
    }
}
