using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Oshinoko.Scripts.Character;

namespace Oshinoko.Scripts.UI;

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeAttack))]
public static class OshinokoCreatureAttackMotionPatch
{
    private static void Prefix(AttackCommand __1)
    {
        OshinokoCreatureCombatMotion.TryGetVisuals(__1.Attacker)?.PlayAttackMotion();
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeDamageReceived))]
public static class OshinokoCreatureHurtMotionPatch
{
    private static void Prefix(Creature __3, decimal __4, Creature? __6)
    {
        if (__4 <= 0m || __6 == null || ReferenceEquals(__3, __6))
        {
            return;
        }

        OshinokoCreatureCombatMotion.TryGetVisuals(__3)?.PlayHurtMotion();
    }
}

internal static class OshinokoCreatureCombatMotion
{
    internal static OshinokoNCreatureVisuals? TryGetVisuals(Creature? creature)
    {
        if (creature?.Player?.Character is not (Aqua or Ruby))
        {
            return null;
        }

        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
        return creatureNode?.Visuals as OshinokoNCreatureVisuals;
    }
}
