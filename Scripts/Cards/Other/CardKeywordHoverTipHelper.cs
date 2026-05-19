using MegaCrit.Sts2.Core.Localization;

namespace Oshinogo.Scripts.Cards.Other;

public static class CardKeywordHoverTipHelper
{
    private static readonly Dictionary<string, Func<IHoverTip>> VanillaPowerTipFactories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["VULNERABLE"] = () => HoverTipFactory.FromPower<VulnerablePower>(),
        ["WEAK"] = () => HoverTipFactory.FromPower<WeakPower>(),
        ["FRAIL"] = () => HoverTipFactory.FromPower<FrailPower>(),
        ["DEBILITATE"] = () => HoverTipFactory.FromPower<DebilitatePower>(),
        ["TRAP"] = () => HoverTipFactory.FromPower<TrapPower>(),
        ["RUMOR"] = () => HoverTipFactory.FromPower<RumorPower>(),
        ["ESCAPE"] = () => HoverTipFactory.FromPower<EscapePower>(),
        ["SHINE"] = () => HoverTipFactory.FromPower<ShinePower>(),
        ["REVENGE"] = () => HoverTipFactory.FromPower<RevengePower>(),
    };

    public static IEnumerable<IHoverTip> Create(params string[] keys)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in keys)
        {
            if (string.IsNullOrWhiteSpace(key) || !seen.Add(key))
            {
                continue;
            }

            if (TryCreateVanillaPowerTip(key, out var vanillaTip))
            {
                yield return vanillaTip;
                continue;
            }

            var title = new LocString("static_hover_tips", $"{key}.title");
            var description = new LocString("static_hover_tips", $"{key}.description");
            yield return new HoverTip(title, description);
        }
    }

    public static IEnumerable<IHoverTip> Merge(IEnumerable<IHoverTip> primary, IEnumerable<IHoverTip> secondary)
    {
        foreach (var tip in primary)
        {
            yield return tip;
        }

        foreach (var tip in secondary)
        {
            yield return tip;
        }
    }

    private static bool TryCreateVanillaPowerTip(string key, out IHoverTip tip)
    {
        tip = null!;
        if (!VanillaPowerTipFactories.TryGetValue(key, out var factory))
        {
            return false;
        }

        tip = factory();
        return true;
    }
}
