using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Oshinoko.Scripts.Relics.Aqua;
using Oshinoko.Scripts.Relics.Ruby;
using STS2RitsuLib;
using STS2RitsuLib.Audio;
using STS2RitsuLib.Interop;
using System.Reflection;

namespace Oshinoko.Scripts;

[ModInitializer(nameof(Init))]
public static class Entry
{
    public const string HarmonyId = "sts2.lingkong.Oshinoko";
    public const string ModId = "Oshinoko";
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

        FmodStudioDeferredBankRegistration.RegisterBank("res://Oshinoko/audios/Oshinoko.bank");
        FmodStudioDeferredBankRegistration.RegisterStudioGuidMappings("res://Oshinoko/audios/GUIDs.txt");



    }
}
