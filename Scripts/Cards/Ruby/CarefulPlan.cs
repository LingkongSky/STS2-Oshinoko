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

// 描述: 查看抽牌堆上的5张牌，选择1张置于抽牌堆顶，下一回合额外获得2点能量和回合闪耀。

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
