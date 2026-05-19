using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Oshinogo.Scripts.Relics.Aqua;
using Oshinogo.Scripts.Relics.Ruby;
using STS2RitsuLib;
using STS2RitsuLib.Audio;
using STS2RitsuLib.Interop;
using System.Reflection;

namespace Oshinogo.Scripts;

[ModInitializer(nameof(Init))]
public static class Entry
{
    public const string HarmonyId = "sts2.lingkong.Oshinogo";
    public const string ModId = "Oshinogo";
    public static readonly MegaCrit.Sts2.Core.Logging.Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        ModConfig.Init();

        var harmony = new Harmony(HarmonyId);
        harmony.PatchAll();


        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<BloodInStage, Sinking>();
        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<IdolPassion, Hope>();
        RitsuLibFramework.RegisterTouchOfOrobasRefinementMapping<Photo, FamilyPhoto>();
        RitsuLibFramework.RegisterTouchOfOrobasRefinementMapping<BrotherWatch, BrotherWatchEX>();

        FmodStudioDeferredBankRegistration.RegisterBank("res://Oshinogo/audios/Oshinogo.bank");
        FmodStudioDeferredBankRegistration.RegisterStudioGuidMappings("res://Oshinogo/audios/GUIDs.txt");



    }
}
