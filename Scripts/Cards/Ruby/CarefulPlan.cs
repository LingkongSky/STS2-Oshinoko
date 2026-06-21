using MegaCrit.Sts2.Core.CardSelection;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: �鿴���ƶ��ϵ�5���ƣ�ѡ��1�����ڳ��ƶѶ�����һ�غ϶�����2�������ͻغ���ҫ��

[RegisterCard(typeof(RubyCardPool))]
public class CarefulPlan : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public CarefulPlan() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var topCards = drawPile.Cards.Take(5).ToList();
        if (topCards.Count > 0)
        {
            var selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                topCards,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();

            if (selected != null)
            {
                await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
            }
        }

        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this, true);
        await PowerCmd.Apply<GainTurnShineNextTurnPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


