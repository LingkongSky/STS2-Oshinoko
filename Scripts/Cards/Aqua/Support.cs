using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 消耗手牌中的虚无牌。
public class Support : AquaCardModel
{
    public Support() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var voidCards = PileType.Hand.GetPile(Owner).Cards.Where(card => card.Keywords.Contains(CardKeyword.Ethereal)).ToList();
        foreach (var card in voidCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



