using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace Oshinogo.Scripts.Patchs;

[HarmonyPatch(typeof(NPotionHolder), nameof(NPotionHolder.ShineOnStartOfCombat))]
public static class NPotionHolderShineSafetyPatch
{
    // 修复potion报错
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
