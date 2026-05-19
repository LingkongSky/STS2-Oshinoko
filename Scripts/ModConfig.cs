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
    public bool EnableHoshinoAiBoss { get; set; } = true;
}

public static class ModConfig
{
    private const string SettingsKey = "settings";
    private static bool _initialized;

    public static bool EnableHoshinoAiBoss
    {
        get
        {
            if (!_initialized)
            {
                return true;
            }

            return RitsuLibFramework.GetDataStore(Entry.ModId).Get<OshinogoSettings>(SettingsKey).EnableHoshinoAiBoss;
        }
        set
        {
            if (!_initialized)
            {
                return;
            }

            var store = RitsuLibFramework.GetDataStore(Entry.ModId);
            store.Modify<OshinogoSettings>(SettingsKey, settings => settings.EnableHoshinoAiBoss = value);
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

        var enableBossBinding = new ModSettingsValueBinding<OshinogoSettings, bool>(
            Entry.ModId,
            SettingsKey,
            SaveScope.Global,
            settings => settings.EnableHoshinoAiBoss,
            (settings, value) => settings.EnableHoshinoAiBoss = value);

        RitsuLibFramework.RegisterModSettings(Entry.ModId, page => page
            .WithTitle(ModSettingsText.Literal("Oshinogo"))
            .WithModDisplayName(ModSettingsText.Literal("Oshinogo"))
            .AddSection("general", section => section
                .WithTitle(ModSettingsText.Literal("General"))
                .AddToggle(
                    "enable_hoshino_ai_boss",
                    ModSettingsText.Literal("Enable Hoshino Ai Boss"),
                    enableBossBinding)));

        _initialized = true;
    }
}
