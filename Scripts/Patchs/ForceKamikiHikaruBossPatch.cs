using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Acts;
using Oshinoko.Scripts.Encounters;

namespace Oshinoko.Scripts.Patchs;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.CreateMap))]
public static class ForceKamikiHikaruBossPatch
{
    public static void Prefix(ActModel __instance)
    {
        if (__instance is not Glory)
        {
            return;
        }

        var mode = ModConfig.KamikiHikaruBossMode;
        if (mode == KamikiHikaruBossMode.Random)
        {
            return;
        }

        var kamikiEncounter = ModelDb.Encounter<KamikiHikaruEncounter>();
        if (kamikiEncounter == null)
        {
            return;
        }

        if (mode == KamikiHikaruBossMode.Forced)
        {
            __instance.SetBossEncounter(kamikiEncounter);

            // At A10 (Double Boss), keep at most one Kamiki in the pair.
            var isDoubleBossAscension = AscensionHelper.GetValueIfAscension(AscensionLevel.DoubleBoss, 1, 0) == 1;
            if (__instance.HasSecondBoss && !isDoubleBossAscension)
            {
                __instance.SetSecondBossEncounter(kamikiEncounter);
            }

            return;
        }

        var nonKamikiBosses = __instance.AllBossEncounters
            .Where(encounter => encounter is not KamikiHikaruEncounter)
            .ToList();
        if (nonKamikiBosses.Count == 0)
        {
            return;
        }

        if (__instance.BossEncounter is KamikiHikaruEncounter)
        {
            __instance.SetBossEncounter(nonKamikiBosses[0]);
        }

        if (__instance.HasSecondBoss && __instance.SecondBossEncounter is KamikiHikaruEncounter)
        {
            var replacement = nonKamikiBosses.FirstOrDefault(encounter => encounter != __instance.BossEncounter) ?? nonKamikiBosses[0];
            __instance.SetSecondBossEncounter(replacement);
        }
    }
}
