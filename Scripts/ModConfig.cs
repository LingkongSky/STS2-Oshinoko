using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace Oshinogo.Scripts;

public enum FjordMosaicMode
{
    Alpha,
    Beta,
    Gamma
}

public sealed class OshinogoSettings
{
    public HoshinoAiBossMode HoshinoAiBossMode { get; set; } = HoshinoAiBossMode.Random;
}

public enum HoshinoAiBossMode
{
    Disabled = 0,
    Random = 1,
    Forced = 2,
}

public static class ModConfig
{
    private const string SettingsKey = "settings";
    private static bool _initialized;

    public static HoshinoAiBossMode HoshinoAiBossMode
    {
        get
        {
            if (!_initialized)
            {
                return HoshinoAiBossMode.Random;
            }

            return RitsuLibFramework.GetDataStore(Entry.ModId).Get<OshinogoSettings>(SettingsKey).HoshinoAiBossMode;
        }
        set
        {
            if (!_initialized)
            {
                return;
            }

            var store = RitsuLibFramework.GetDataStore(Entry.ModId);
            store.Modify<OshinogoSettings>(SettingsKey, settings => settings.HoshinoAiBossMode = value);
            store.Save(SettingsKey);
        }
    }

    public static void Init()
    {
        if (_initialized)
        {
            return;
        }

        using (RitsuLibFramework.BeginModDataRegistration(Entry.ModId))
        {
            var store = RitsuLibFramework.GetDataStore(Entry.ModId);
            store.Register(
                key: SettingsKey,
                fileName: "settings.json",
                scope: SaveScope.Global,
                defaultFactory: () => new OshinogoSettings(),
                autoCreateIfMissing: true);
        }

        var bossModeBinding = new ModSettingsValueBinding<OshinogoSettings, HoshinoAiBossMode>(
            Entry.ModId,
            SettingsKey,
            SaveScope.Global,
            settings => settings.HoshinoAiBossMode,
            (settings, value) => settings.HoshinoAiBossMode = value);

        RitsuLibFramework.RegisterModSettings(Entry.ModId, page => page
            .WithTitle(ModSettingsText.Literal("Oshinogo"))
            .WithModDisplayName(ModSettingsText.Literal("Oshinogo"))
            .AddSection("general", section => section
                .WithTitle(ModSettingsText.Literal("General"))
                .AddEnumChoice(
                    "hoshino_ai_boss_mode",
                    ModSettingsText.Literal("Act 2 Hoshino Ai Boss Mode"),
                    bossModeBinding,
                    mode => mode switch
                    {
                        HoshinoAiBossMode.Disabled => ModSettingsText.Literal("Disabled"),
                        HoshinoAiBossMode.Random => ModSettingsText.Literal("Random"),
                        HoshinoAiBossMode.Forced => ModSettingsText.Literal("Forced"),
                        _ => ModSettingsText.Literal("Disabled"),
                    })));

        _initialized = true;
    }
}
