using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using Oshinoko.Scripts.Encounters;
using STS2RitsuLib;

namespace Oshinoko.Scripts;

internal static class BossEncounterCoordinator
{
    private static IDisposable? _actEnteringSubscription;

    public static void Init()
    {
        _actEnteringSubscription ??= RitsuLibFramework.SubscribeLifecycle<ActEnteringEvent>(
            OnActEntering,
            replayCurrentState: false);
    }

    private static void OnActEntering(ActEnteringEvent evt)
    {
        RunState? runState = evt.RunManager.DebugOnlyGetState();
        if (runState == null || evt.TargetActIndex < 0 || evt.TargetActIndex >= runState.Acts.Count)
        {
            return;
        }

        ActModel act = runState.Acts[evt.TargetActIndex];
        switch (act)
        {
            case Hive hive:
                ConfigureHoshinoAiBoss(hive, runState);
                break;
            case Glory glory:
                ConfigureKamikiHikaruBoss(glory, runState);
                break;
        }
    }

    private static void ConfigureHoshinoAiBoss(Hive act, RunState runState)
    {
        var aiEncounter = ModelDb.Encounter<AiEncounter>();
        if (aiEncounter == null)
        {
            return;
        }

        var settings = ModConfig.GetBossSettingsForRun(runState);
        var mode = settings.HoshinoAiBossMode;
        if (!ModConfig.ShouldIncludeModBosses(runState, settings))
        {
            mode = HoshinoAiBossMode.Disabled;
        }

        if (mode == HoshinoAiBossMode.Random)
        {
            return;
        }

        if (mode == HoshinoAiBossMode.Forced)
        {
            act.SetBossEncounter(aiEncounter);
            if (act.HasSecondBoss)
            {
                act.SetSecondBossEncounter(aiEncounter);
            }

            return;
        }

        var nonAiBosses = act.AllBossEncounters
            .Where(encounter => encounter is not AiEncounter)
            .ToList();
        if (nonAiBosses.Count == 0)
        {
            return;
        }

        if (act.BossEncounter is AiEncounter)
        {
            act.SetBossEncounter(nonAiBosses[0]);
        }

        if (act.HasSecondBoss && act.SecondBossEncounter is AiEncounter)
        {
            var replacement = nonAiBosses.FirstOrDefault(encounter => encounter != act.BossEncounter)
                ?? nonAiBosses[0];
            act.SetSecondBossEncounter(replacement);
        }
    }

    private static void ConfigureKamikiHikaruBoss(Glory act, RunState runState)
    {
        var kamikiEncounter = ModelDb.Encounter<KamikiHikaruEncounter>();
        if (kamikiEncounter == null)
        {
            return;
        }

        var settings = ModConfig.GetBossSettingsForRun(runState);
        var mode = settings.KamikiHikaruBossMode;
        if (!ModConfig.ShouldIncludeModBosses(runState, settings))
        {
            mode = KamikiHikaruBossMode.Disabled;
        }

        if (mode == KamikiHikaruBossMode.Random)
        {
            return;
        }

        if (mode == KamikiHikaruBossMode.Forced)
        {
            act.SetBossEncounter(kamikiEncounter);

            // At A10 (Double Boss), keep at most one Kamiki in the pair.
            var isDoubleBossAscension =
                AscensionHelper.GetValueIfAscension(AscensionLevel.DoubleBoss, 1, 0) == 1;
            if (act.HasSecondBoss && !isDoubleBossAscension)
            {
                act.SetSecondBossEncounter(kamikiEncounter);
            }

            return;
        }

        var nonKamikiBosses = act.AllBossEncounters
            .Where(encounter => encounter is not KamikiHikaruEncounter)
            .ToList();
        if (nonKamikiBosses.Count == 0)
        {
            return;
        }

        if (act.BossEncounter is KamikiHikaruEncounter)
        {
            act.SetBossEncounter(nonKamikiBosses[0]);
        }

        if (act.HasSecondBoss && act.SecondBossEncounter is KamikiHikaruEncounter)
        {
            var replacement = nonKamikiBosses.FirstOrDefault(encounter => encounter != act.BossEncounter)
                ?? nonKamikiBosses[0];
            act.SetSecondBossEncounter(replacement);
        }
    }
}
