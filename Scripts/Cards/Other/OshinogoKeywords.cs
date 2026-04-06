using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace Oshinogo.Scripts.Cards.Other
{
    public class OshinogoKeywords
    {
        // 自定义枚举的名字。最终会变成{前缀}-{枚举值大写}的形式，例如TEST-UNIQUE
        [CustomEnum("SHINE")]
        // 放在原版卡牌描述的位置，这里是卡牌描述的前面
        [KeywordProperties(AutoKeywordPosition.After)]
        public static CardKeyword Shine;
    }

}
