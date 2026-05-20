using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Acts;
using Oshinogo.Scripts.Encounters;

namespace Oshinogo.Scripts.Patchs;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]
public static class ForceHoshinoAiBossPatch
{
    public static void Prefix(ActModel __instance)
    {
        if (__instance is not Hive)
        {
            return;
        }

        var mode = ModConfig.HoshinoAiBossMode;
        if (mode == HoshinoAiBossMode.Random)
        {
            return;
        }

        var aiEncounter = ModelDb.Encounter<AiEncounter>();
        if (aiEncounter == null)
        {
            return;
        }

        if (mode == HoshinoAiBossMode.Forced)
        {
            __instance.SetBossEncounter(aiEncounter);
            if (__instance.HasSecondBoss)
            {
                __instance.SetSecondBossEncounter(aiEncounter);
            }

            return;
        }

        var nonAiBosses = __instance.AllBossEncounters
            .Where(encounter => encounter is not AiEncounter)
            .ToList();
        if (nonAiBosses.Count == 0)
        {
            return;
        }

        if (__instance.BossEncounter is AiEncounter)
        {
            __instance.SetBossEncounter(nonAiBosses[0]);
        }

        if (__instance.HasSecondBoss && __instance.SecondBossEncounter is AiEncounter)
        {
            var replacement = nonAiBosses.FirstOrDefault(encounter => encounter != __instance.BossEncounter) ?? nonAiBosses[0];
            __instance.SetSecondBossEncounter(replacement);
        }
    }
}
