using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Extensions;

namespace Oshinogo.Scripts.Cards.Other
{
    public class ShineDymicVar : DynamicVar
    {
        // 在描述中用作占位符的键，推荐添加前缀避免撞车
        public const string Key = "Shine";

        public static readonly string LocKey = Key.ToUpperInvariant();

        public ShineDymicVar(decimal baseValue) : base(Key, baseValue)
        {
            this.WithTooltip(LocKey);
        }
    }

}