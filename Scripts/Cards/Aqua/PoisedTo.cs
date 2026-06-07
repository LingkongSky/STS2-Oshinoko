using MegaCrit.Sts2.Core.CardSelection;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 造成10(14)点伤害，选择一张手牌给予虚无。
public class PoisedTo : AquaCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public PoisedTo() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (PileType.Hand.GetPile(Owner).Cards.Count == 0)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1),
            card => !card.GetKeywordsWithSources(KeywordSources.Local).Contains(CardKeyword.Ethereal),
            this)).FirstOrDefault();

        if (selected != null)
        {
            CardCmd.ApplyKeyword(selected, CardKeyword.Ethereal);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}




