using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Other
{
    public class ShineDymicVar : DynamicVar
    {
        // 鍦ㄦ弿杩颁腑鐢ㄤ綔鍗犱綅绗︾殑閿紝鎺ㄨ崘娣诲姞鍓嶇紑閬垮厤鎾炶溅
        public const string Key = "Shine";

        public static readonly string LocKey = Key.ToUpperInvariant();

        public ShineDymicVar(decimal baseValue) : base(Key, baseValue)
        {
            this.WithTooltip(LocKey);
        }
    }

}
