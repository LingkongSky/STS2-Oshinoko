using MegaCrit.Sts2.Core.CardSelection;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 选择两张手牌，本回合内降低1点费用。
public class GiveHope : AquaCardModel
{
    public GiveHope() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner == null)
        {
            return;
        }

        var handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        if (handCount <= 0)
        {
            return;
        }

        var count = Math.Min(2, handCount);
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, count),
            _ => true,
            this);

        foreach (var card in selected)
        {
            card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
