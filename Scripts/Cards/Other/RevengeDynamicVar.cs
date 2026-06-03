
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinoko.Scripts.Cards.Other;

public class RevengeDynamicVar : DynamicVar
{
    public const string Key = "Revenge";

    public static readonly string LocKey = Key.ToUpperInvariant();

    public RevengeDynamicVar(decimal baseValue) : base(Key, baseValue)
    {}
}




