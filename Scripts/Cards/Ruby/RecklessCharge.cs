using MegaCrit.Sts2.Core.CardSelection;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

[RegisterCard(typeof(RubyCardPool))]
public class RecklessCharge : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
    ];

    public RecklessCharge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(2)
            .Execute(choiceContext);

        if (Owner == null)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
            _ => true,
            this)).FirstOrDefault();

        if (selected == null)
        {
            return;
        }

        var cost = selected.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (cost < 0)
        {
            cost = 0;
        }

        await CardCmd.Exhaust(choiceContext, selected);
        await RevengePowerHelper.ApplyRevenge(Owner.Creature, cost, ValueDuration.Temp, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
