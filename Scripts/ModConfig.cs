using MegaCrit.Sts2.Core.Runs;
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

public enum ModBossAppearanceMode
{
    Disabled = 0,
    OnlyWithAquaOrRuby = 1,
    Always = 2,
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

public sealed class OshinokoSettings
{
    public ModBossAppearanceMode BossAppearanceMode { get; set; } = ModBossAppearanceMode.OnlyWithAquaOrRuby;
    public HoshinoAiBossMode HoshinoAiBossMode { get; set; } = HoshinoAiBossMode.Random;
    public KamikiHikaruBossMode KamikiHikaruBossMode { get; set; } = KamikiHikaruBossMode.Random;
}

public static class ModConfig
{
    private const string SettingsKey = "settings";
    private static bool _initialized;
    private static I18N? _settingsI18N;

    private static class ModSettingsLocalization
    {
        public static I18N I18N => _settingsI18N ??= RitsuLibFramework.CreateModLocalization(
            modId: Entry.ModId,
            instanceName: $"{Entry.ModId}.settings",
            pckFolders: ["res://Oshinoko/localization"]);

        public static ModSettingsText Text(string key, string fallback)
            => ModSettingsText.I18N(I18N, key, fallback);
    }

    public static ModBossAppearanceMode BossAppearanceMode
    {
        get
        {
            if (!_initialized)
            {
                return ModBossAppearanceMode.OnlyWithAquaOrRuby;
            }

            return RitsuLibFramework.GetDataStore(Entry.ModId).Get<OshinokoSettings>(SettingsKey).BossAppearanceMode;
        }
        set
        {
            if (!_initialized)
            {
                return;
            }

            var store = RitsuLibFramework.GetDataStore(Entry.ModId);
            store.Modify<OshinokoSettings>(SettingsKey, settings => settings.BossAppearanceMode = value);
            store.Save(SettingsKey);
        }
    }

    public static bool ShouldIncludeModBosses(RunState runState)
    {
        return BossAppearanceMode switch
        {
            ModBossAppearanceMode.Disabled => false,
            ModBossAppearanceMode.Always => true,
            ModBossAppearanceMode.OnlyWithAquaOrRuby => runState.Players.Any(HasAquaOrRubyCharacter),
            _ => false,
        };
    }

    public static HoshinoAiBossMode HoshinoAiBossMode
    {
        get
        {
            if (!_initialized)
            {
                return Oshinoko.Scripts.HoshinoAiBossMode.Random;
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
                return Oshinoko.Scripts.KamikiHikaruBossMode.Random;
            }

            return RitsuLibFramework.GetDataStore(Entry.ModId).Get<OshinokoSettings>(SettingsKey)
                .KamikiHikaruBossMode;
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

        var bossAppearanceModeBinding = new ModSettingsValueBinding<OshinokoSettings, ModBossAppearanceMode>(
            Entry.ModId,
            SettingsKey,
            SaveScope.Global,
            settings => settings.BossAppearanceMode,
            (settings, value) => settings.BossAppearanceMode = value);

        var hoshinoAiBossModeBinding = new ModSettingsValueBinding<OshinokoSettings, HoshinoAiBossMode>(
            Entry.ModId,
            SettingsKey,
            SaveScope.Global,
            settings => settings.HoshinoAiBossMode,
            (settings, value) => settings.HoshinoAiBossMode = value);

        var kamikiHikaruBossModeBinding = new ModSettingsValueBinding<OshinokoSettings, KamikiHikaruBossMode>(
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
                    "boss_appearance_mode",
                    ModSettingsLocalization.Text("boss_appearance_mode.label", "Mod Boss Appearance"),
                    bossAppearanceModeBinding,
                    mode => mode switch
                    {
                        ModBossAppearanceMode.Disabled => ModSettingsLocalization.Text(
                            "boss_appearance_mode.disabled",
                            "Disabled"),
                        ModBossAppearanceMode.OnlyWithAquaOrRuby => ModSettingsLocalization.Text(
                            "boss_appearance_mode.only_with_aqua_or_ruby",
                            "Only With Aqua/Ruby"),
                        ModBossAppearanceMode.Always => ModSettingsLocalization.Text(
                            "boss_appearance_mode.always",
                            "All Characters"),
                        _ => ModSettingsLocalization.Text("boss_appearance_mode.disabled", "Disabled"),
                    })
                .AddEnumChoice(
                    "hoshino_ai_boss_mode",
                    ModSettingsLocalization.Text("hoshino_ai_boss_mode.label", "Act 2 Hoshino Ai Boss Mode"),
                    hoshinoAiBossModeBinding,
                    mode => mode switch
                    {
                        HoshinoAiBossMode.Disabled => ModSettingsLocalization.Text(
                            "hoshino_ai_boss_mode.disabled",
                            "Disabled"),
                        HoshinoAiBossMode.Random => ModSettingsLocalization.Text(
                            "hoshino_ai_boss_mode.random",
                            "Random"),
                        HoshinoAiBossMode.Forced => ModSettingsLocalization.Text(
                            "hoshino_ai_boss_mode.forced",
                            "Forced"),
                        _ => ModSettingsLocalization.Text("hoshino_ai_boss_mode.disabled", "Disabled"),
                    })
                .AddEnumChoice(
                    "kamiki_hikaru_boss_mode",
                    ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.label", "Act 3 Kamiki Hikaru Boss Mode"),
                    kamikiHikaruBossModeBinding,
                    mode => mode switch
                    {
                        KamikiHikaruBossMode.Disabled => ModSettingsLocalization.Text(
                            "kamiki_hikaru_boss_mode.disabled",
                            "Disabled"),
                        KamikiHikaruBossMode.Random => ModSettingsLocalization.Text(
                            "kamiki_hikaru_boss_mode.random",
                            "Random"),
                        KamikiHikaruBossMode.Forced => ModSettingsLocalization.Text(
                            "kamiki_hikaru_boss_mode.forced",
                            "Forced"),
                        _ => ModSettingsLocalization.Text("kamiki_hikaru_boss_mode.disabled", "Disabled"),
                    })));

        _initialized = true;
    }

    private static bool HasAquaOrRubyCharacter(Player player)
    {
        return player.Character is Oshinoko.Scripts.Character.Aqua or Oshinoko.Scripts.Character.Ruby;
    }
}
