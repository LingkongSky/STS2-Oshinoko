using MegaCrit.Sts2.Core.Localization;

namespace Oshinoko.Scripts.RestSite;

public static class OshinokoRestSiteLocalization
{
    private const string TableId = "rest_site_ui";

    public static LocString Key(string key) => new(TableId, key);
}
