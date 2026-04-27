using System;
using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 选择两张手牌，本回合内降低1点费用。
public class GiveHope : AquaCardModel
{
    public GiveHope() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        if (handCount <= 0)
        {
            return;
        }

        var count = Math.Min(2, handCount);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, count);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, _ => true, this);
        foreach (var card in selected.Take(count))
        {
            card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
