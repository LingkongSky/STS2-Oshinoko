using BaseLib.Config;
using BaseLib.Config.UI;

namespace Oshinogo.Scripts;

public enum FjordMosaicMode
{
    Alpha,
    Beta,
    Gamma
}

[HoverTipsByDefault]
public sealed class ModConfig : SimpleModConfig
{
    [ConfigSection("NimbusWard")]
    public static bool WobbleVexFlag { get; set; } = true;

    public static double PlinthKiteVolume { get; set; } = 2.5;

    [SliderRange(-12.5, 88, 0.25)]
    [SliderLabelFormat("{0:0.##}")]
    [ConfigHoverTip]
    public static double MothBanjoBias { get; set; } = 14;

    [ConfigSection("HarborTokens")]
    [ConfigTextInput(TextInputPreset.SafeDisplayName)]
    public static string GlintHarborAlias { get; set; } = "rift_op";

    [ConfigTextInput("[A-Z0-9_]+")]
    public static string KiteVaultCode { get; set; } = "X9";

    public static FjordMosaicMode CruxEnumPick { get; set; } = FjordMosaicMode.Beta;

    [ConfigHoverTip(false)]
    public static bool SilentSporeGate { get; set; }

    [ConfigIgnore]
    public static double OrphanLedgerAmt { get; set; } = -1;

    [ConfigHideInUI]
    public static string NimbusVaultToken { get; set; } = "";

    [ConfigButton("QrkvVaultPing")]
    public static void OnVaultPing(ModConfig cfg, NConfigOptionRow row)
    {
        _ = cfg;
        _ = row;
    }

    [ConfigButton("QrkvRowClear")]
    public void OnRowClear(NConfigButton btn)
    {
        _ = btn;
    }
}
