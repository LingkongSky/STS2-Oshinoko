using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Oshinoko.Scripts.Encounters;

namespace Oshinoko.Scripts.Compatibility;

// DominionModel does not exist in 0.111. Keep it out of this assembly's type
// references so the same mod DLL can still be discovered and loaded there.
public static class BossDominionCompat
{
    private static readonly Lazy<Type[]> DominionTypes = new(CreateDominionTypes);
    internal static readonly ConditionalWeakTable<PowerModel, AbstractModel> OpeningRegeneration = new();

    internal static void Initialize()
    {
        if (DominionTypes.Value.Length > 0)
        {
            // Associate before AssemblyInfo and the network ID cache are built.
            typeof(ModManager).GetMethod("AssociateAssemblyWithMod")!
                .Invoke(null, [Entry.ModId, DominionTypes.Value[0].Assembly]);
        }
    }

    internal static void RegisterModels()
    {
        foreach (var type in DominionTypes.Value)
        {
            ModelDb.Inject(type);
        }
    }

    internal static AbstractModel? GetDominion(EncounterModel encounter)
    {
        var index = encounter switch
        {
            AiEncounter => 0,
            KamikiHikaruEncounter => 1,
            _ => -1,
        };
        if (index < 0 || DominionTypes.Value.Length == 0)
        {
            return null;
        }

        return ModelDb.GetByIdOrNull<AbstractModel>(ModelDb.GetId(DominionTypes.Value[index]));
    }

    public static async Task AfterRoomEntered(AbstractModel dominion, AbstractRoom room)
    {
        if (room is not CombatRoom combatRoom)
        {
            return;
        }

        var targets = combatRoom.CombatState.Creatures;
        if (dominion.GetType() == DominionTypes.Value[0])
        {
            foreach (var target in targets)
            {
                var power = ModelDb.Power<RegenPower>().ToMutable();
                OpeningRegeneration.Add(power, dominion);
                try
                {
                    await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), power, target, 5, null, null);
                }
                finally
                {
                    OpeningRegeneration.Remove(power);
                }
            }
        }
        else
        {
            await PowerCmd.Apply<DebilitatePower>(new ThrowingPlayerChoiceContext(), targets, 1, null, null);
            await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, 1, null, null);
        }
    }

    public static IEnumerable<IHoverTip> GetExtraHoverTips(AbstractModel dominion)
    {
        if (dominion.GetType() == DominionTypes.Value[0])
        {
            return HoverTipFactory.FromPowerWithPowerHoverTips<RegenPower>(5);
        }

        return HoverTipFactory.FromPowerWithPowerHoverTips<DebilitatePower>(1)
            .Concat(HoverTipFactory.FromPowerWithPowerHoverTips<VulnerablePower>(1));
    }

    private static Type[] CreateDominionTypes()
    {
        var baseType = typeof(AbstractModel).Assembly.GetType("MegaCrit.Sts2.Core.Models.Dread.DominionModel");
        if (baseType == null)
        {
            return [];
        }

        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Oshinoko.Dread"), AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule("Oshinoko.Dread");
        var types = new List<Type>();
        foreach (var name in new[] { "HoshinoAiDominion", "KamikiHikaruDominion" })
        {
            var type = module.DefineType($"Oshinoko.Scripts.Dread.{name}", TypeAttributes.Public | TypeAttributes.Sealed, baseType);
            type.DefineDefaultConstructor(MethodAttributes.Public);
            foreach (var (methodName, callbackName) in new[]
            {
                (nameof(AbstractModel.AfterRoomEntered), nameof(AfterRoomEntered)),
                ("get_ExtraHoverTips", nameof(GetExtraHoverTips)),
            })
            {
                var original = baseType.GetMethod(methodName)!;
                var parameters = original.GetParameters();
                var method = type.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    original.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                for (var i = 0; i < parameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i + 1);
                }
                il.Emit(OpCodes.Call, typeof(BossDominionCompat).GetMethod(callbackName)!);
                il.Emit(OpCodes.Ret);
                type.DefineMethodOverride(method, original);
            }
            types.Add(type.CreateType()!);
        }

        return types.ToArray();
    }
}

// Regen normally scales on enemies in multiplayer. Only the instances being
// applied by Ai's opening effect must keep the specified five stacks.
[HarmonyPatch(typeof(PowerModel), nameof(PowerModel.GetScaledAmountForMultiplayer))]
internal static class BossDominionRegenScalingPatch
{
    private static bool Prefix(PowerModel __instance, decimal amount, ref decimal __result)
    {
        if (!BossDominionCompat.OpeningRegeneration.TryGetValue(__instance, out _))
        {
            return true;
        }

        __result = amount;
        return false;
    }
}

// Inject before the game's network model-ID cache is initialized. This also
// restores the models after ModelDb.ResetForTest followed by Init.
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
internal static class BossDominionRegistrationPatch
{
    private static void Postfix() => BossDominionCompat.RegisterModels();
}

[HarmonyPatch]
internal static class BossEncounterDominionPatch
{
    private static bool Prepare() => typeof(EncounterModel).GetMethod("GetDominion") != null;

    private static MethodBase TargetMethod() => typeof(EncounterModel).GetMethod("GetDominion")!;

    private static void Postfix(EncounterModel __instance, ref object? __result)
    {
        if (__instance is AiEncounter or KamikiHikaruEncounter)
        {
            __result = BossDominionCompat.GetDominion(__instance);
        }
    }
}
