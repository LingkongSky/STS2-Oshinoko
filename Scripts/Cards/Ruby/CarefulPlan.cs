using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鏌ョ湅鎶界墝鍫嗕笂鐨?寮犵墝锛岄€夋嫨1寮犵疆浜庢娊鐗屽爢椤讹紝涓嬩竴鍥炲悎棰濆鑾峰緱1鐐硅兘閲忓拰鍥炲悎闂€€銆?

[Pool(typeof(RubyCardPool))]
public class CarefulPlan : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    public CarefulPlan() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var topCards = drawPile.Cards.Take(5).ToList();
        if (topCards.Count > 0)
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
            var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, topCards, Owner, prefs)).FirstOrDefault();
            if (selected != null)
            {
                await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
            }
        }

        await PowerCmd.Apply<EnergyNextTurnPower>(Owner.Creature, 2, Owner.Creature, this);
        await PowerCmd.Apply<GainTurnShineNextTurnPower>(Owner.Creature, 2, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
