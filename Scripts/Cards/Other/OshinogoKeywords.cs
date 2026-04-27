using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;


namespace Oshinogo.Scripts.Cards.Other
{
    public class OshinogoKeywords
    {
        // 鑷畾涔夋灇涓剧殑鍚嶅瓧銆傛渶缁堜細鍙樻垚{鍓嶇紑}-{鏋氫妇鍊煎ぇ鍐檥鐨勫舰寮忥紝渚嬪TEST-UNIQUE
        [CustomEnum("SHINE")]
        // 鏀惧湪鍘熺増鍗＄墝鎻忚堪鐨勪綅缃紝杩欓噷鏄崱鐗屾弿杩扮殑鍓嶉潰
        [KeywordProperties(AutoKeywordPosition.After)]
        public static CardKeyword Shine;
    }

}
