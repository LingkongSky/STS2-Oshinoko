using HarmonyLib;
using Oshinoko.Scripts.Monsters;

namespace Oshinoko.Scripts.Patchs;

// Both game versions keep these argument names; 0.113 adds a source argument.
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterDeath))]
internal static class KamikiHikaruDeathPatch
{
    private static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext,
        Creature creature, bool wasRemovalPrevented, float deathAnimLength, ref Task __result)
    {
        if (__instance is not KamikiHikaru boss)
        {
            return true;
        }

        __result = boss.AfterDeathCompat(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
        return false;
    }
}
