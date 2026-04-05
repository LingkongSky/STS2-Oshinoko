using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics
{
    // 加入哪个遗物池，此处为通用
    [Pool(typeof(RubyRelicPool))]
    public class Photo : CustomRelicModel
    {
        // 稀有度
        public override RelicRarity Rarity => RelicRarity.Common;

        // 遗物的数值。替换本地化中的{Cards}。
        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

        // 小图标
        public override string PackedIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";
        // 轮廓图标
        protected override string PackedIconOutlinePath => $"res://Oshinogo/images/relics/{GetType().Name}.png";
        // 大图标
        protected override string BigIconPath => $"res://Oshinogo/images/relics/{GetType().Name}.png";

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            // 这里的DynamicVars.Cards.IntValue为上面设置的CardsVar的数值。
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
        }
    }
}
