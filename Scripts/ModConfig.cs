using System.Reflection;
using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;

namespace Oshinoko.Scripts;

public enum FjordMosaicMode
{
    Alpha,
    Beta,
    Gamma
}

public sealed class OshinokoSettings
{
    public HoshinoAiBossMode HoshinoAiBossMode { get; set; } = HoshinoAiBossMode.Random;
    public KamikiHikaruBossMode KamikiHikaruBossMode { get; set; } = KamikiHikaruBossMode.Random;
}

public enum HoshinoAiBossMode
{
    Disabled = 0,
    Random = 1,
    Forced = 2,
}

public enum KamikiHikaruBossMode
{
    Disabled = 0,
    Random = 1,
    Forced = 2,
}

public static class ModConfig
{
    private const string SettingsKey = "settings";
    private static bool _initialized;

    private static class ModSettingsLocalization
    {
        private static readonly Lazy<I18N> InstanceFactory = new(() => new I18N(
            "Oshinoko-ModSettings",
            resourceFolders: ["Oshinoko.Settings.Localization.ModSettingsUi"],
            resourceAssembly: Assembly.GetExecutingAssembly()));

        public static I18N Instance => InstanceFactory.Value;

        public static ModSettingsText Text(string key, string fallback)
            => ModSettingsText.I18N(Instance, key, fallback);
    }

    public static HoshinoAiBossMode HoshinoAiBossMode
    {
        get
        {
            if (!_initialized)
            {
                return HoshinoAiBossMode.Random;
            }

            return RitsuLibFramework.GetDataStore(Entry.ModId).Get<OshinokoSettings>(SettingsKey).HoshinoAiBossMode;
        }
        set
        {
            if (!_initialized)
            {
                return;
            }

            var store = RitsuLibFramework.GetDataStore(Entry.ModId);
            store.Modify<OshinokoSettings>(SettingsKey, settings => settings.HoshinoAiBossMode = value);
            store.Save(SettingsKey);
        }
    }

    public static KamikiHikaruBossMode KamikiHikaruBossMode
    {
        get
        {
            if (!_initialized)
            {
                return KamikiHikaruBossMode.Random;
            }

            return RitsuLibFramework.GetDataStore(Entry.ModId).Get<OshinokoSettings>(SettingsKey).KamikiHikaruBossMode;
        }
        set
        {
            if (!_initialized)
            {
                return;
            }

            var store = RitsuLibFramework.GetDataStore(Entry.ModId);
            store.Modify<OshinokoSettings>(SettingsKey, settings => settings.KamikiHikaruBossMode = value);
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
                defaultFactory: () => new OshinokoSettings(),
                autoCreateIfMissing: true);
        }

        var hoshinoAiModeBinding = new ModSettingsValueBinding<OshinokoSettings, HoshinoAiBossMode>(
            Entry.ModId,
            SettingsKey,
            SaveScope.Global,
            settings => settings.HoshinoAiBossMode,
            (settings, value) => settings.HoshinoAiBossMode = value);

        var kamikiHikaruModeBinding = new ModSettingsValueBinding<OshinokoSettings, KamikiHikaruBossMode>(
            Entry.ModId,
            SettingsKey,
            SaveScope.Global,
            settings => settings.KamikiHikaruBossMode,
            (settings, value) => settings.KamikiHikaruBossMode = value);

        RitsuLibFramework.RegisterModSettings(Entry.ModId, page => page
            .WithTitle(ModSettingsLocalization.Text("oshinoko.title", "Oshinoko"))
            .WithModDisplayName(ModSettingsLocalization.Text("oshinoko.display_name", "Oshinoko"))
            .AddSection("general", section => section
                .WithTitle(ModSettingsLocalization.Text("general.title", "General"))
                .AddEnumChoice(
                    "hoshino_ai_boss_mode",
                    ModSettingsLocalization.Text("hoshino_ai_boss_mode.label", "Act 2 Hoshino Ai Boss Mode"),
                    hoshinoAiModeBinding,
                    mode => mode switch
                    {
                        HoshinoAiBossMode.Disabled => ModSettingsLocalization.Text("hoshino_ai_boss_mode.disabled", "Disabled"),
                        HoshinoAiBossMode.Random => ModSettingsLocalization.Text("hoshino_ai_boss_mode.random", "Random"),
                        HoshinoAiBossMode.Forced => ModSettingsLocalization.Text("hoshino_ai_boss_mode.forced", "Forced"),
                        _ => ModSettingsLocalization.Text("hoshino_ai_boss_mode.disabled", "Disabled"),
                    })
                .AddEnumChoice(
                    "kamiki_hikaru_boss_mode",
                    ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.label", "Act 3 Kamiki Hikaru Boss Mode"),
                    kamikiHikaruModeBinding,
                    mode => mode switch
                    {
                        KamikiHikaruBossMode.Disabled => ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.disabled", "Disabled"),
                        KamikiHikaruBossMode.Random => ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.random", "Random"),
                        KamikiHikaruBossMode.Forced => ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.forced", "Forced"),
                        _ => ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.disabled", "Disabled"),
                    })));

        _initialized = true;
    }
}
