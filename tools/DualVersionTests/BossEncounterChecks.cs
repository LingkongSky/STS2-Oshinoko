using System.Reflection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;

internal static class BossEncounterChecks
{
    internal static void Run(Assembly mod)
    {
        var kamikiType = mod.GetType("Oshinoko.Scripts.Encounters.KamikiHikaruEncounter", true)!;
        var force = mod.GetType("Oshinoko.Scripts.BossEncounterCoordinator", true)!
            .GetMethod("SetForcedKamikiBoss", BindingFlags.Static | BindingFlags.NonPublic)!;
        ModelDb.ResetForTest();
        ModelDb.Init([typeof(Glory), typeof(QueenBoss), typeof(TestSubjectBoss), kamikiType]);
        var assertions = 0;
        void Check(bool condition, string message)
        {
            assertions++;
            if (!condition) throw new InvalidOperationException(message);
        }
        try
        {
            var queen = ModelDb.Encounter<QueenBoss>();
            var testSubject = ModelDb.Encounter<TestSubjectBoss>();
            var kamiki = ModelDb.GetByIdOrNull<EncounterModel>(ModelDb.GetId(kamikiType))!;
            EncounterModel[] bosses = [queen, testSubject, kamiki];
            foreach (var first in bosses)
            foreach (var second in new EncounterModel?[] { null, queen, testSubject, kamiki })
            {
                var act = ModelDb.Act<Glory>().ToMutable();
                typeof(ActModel).GetField("_allBossEncounters", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .SetValue(act, bosses);
                act.SetBossEncounter(first);
                act.SetSecondBossEncounter(second);
                var expectedFirst = second == null ? kamiki
                    : first != kamiki ? first
                    : second != kamiki ? second
                    : queen;
                var expectedSecond = second == null ? null : kamiki;
                // Act entry and map creation can both enforce the same setting.
                for (var pass = 0; pass < 2; pass++)
                {
                    force.Invoke(null, [act, kamiki]);
                    Check(act.BossEncounter.Id == expectedFirst.Id,
                        $"Forced Kamiki first slot: initial={first.Id}, second={second?.Id}, pass={pass}");
                    Check(act.SecondBossEncounter?.Id == expectedSecond?.Id,
                        $"Forced Kamiki second slot: initial={first.Id}, second={second?.Id}, pass={pass}");
                    Check(act.HasSecondBoss == (second != null), "Single/double boss count preserved");
                    Check(act.BossEncounter.Id != act.SecondBossEncounter?.Id, "Kamiki never occupies both slots");
                }
            }
            Console.WriteLine($"BOSS_SLOTS: {assertions} assertions; single boss, forced second slot, first-slot swap, duplicate repair and repeated enforcement");
        }
        finally
        {
            ModelDb.ResetForTest();
        }
    }
}
