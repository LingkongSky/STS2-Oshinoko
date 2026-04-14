using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs.History;
using Oshinogo.Scripts.Relics.Ruby;

namespace Oshinogo.Scripts.RestSite;

public sealed class BKomachiGathering : RestSiteOption
{
    public const string OptionIdValue = "BKOMACHI";
    public const string IconPath = "res://Oshinogo/images/ui/rest_site/option_bkomachi.png";
    private const int MaxUses = 3;

    private readonly int _useCount;

    public override string OptionId => OptionIdValue;

    public override LocString Description
    {
        get
        {
            if (!IsEnabled)
            {
                return new LocString("rest_site_ui", "OPTION_" + OptionId + ".descriptionDisabled");
            }

            string key = _useCount switch
            {
                0 => "description1",
                1 => "description2",
                2 => "description3",
                _ => "descriptionDisabled"
            };
            return new LocString("rest_site_ui", "OPTION_" + OptionId + "." + key);
        }
    }

    public BKomachiGathering(Player owner)
        : base(owner)
    {
        _useCount = GetUsageCount(owner);
        IsEnabled = _useCount < MaxUses;
    }

    public override IEnumerable<string> AssetPaths => base.AssetPaths;

    public override async Task<bool> OnSelect()
    {
        if (_useCount >= MaxUses)
        {
            return false;
        }

        switch (_useCount)
        {
            case 0:
                await RelicCmd.Obtain<KanaArima>(Owner);
                break;
            case 1:
                await RelicCmd.Obtain<Mem>(Owner);
                break;
            case 2:
                await RelicCmd.Obtain<NewBKomachi>(Owner);
                break;
        }

        return true;
    }

    private static int GetUsageCount(Player player)
    {
        int count = 0;
        foreach (IReadOnlyList<MapPointHistoryEntry> actHistory in player.RunState.MapPointHistory)
        {
            foreach (MapPointHistoryEntry entry in actHistory)
            {
                if (entry.MapPointType != MapPointType.RestSite)
                {
                    continue;
                }

                var playerEntry = entry.GetEntry(player.NetId);
                foreach (string choice in playerEntry.RestSiteChoices)
                {
                    if (choice == OptionIdValue)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }
}
