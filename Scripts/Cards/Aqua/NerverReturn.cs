using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// ����: �������е������ƣ�ÿ����һ�Ŷ����е������4(6)���˺���
public class NerverReturn : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];

    public NerverReturn() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var voidCards = PileType.Hand.GetPile(Owner).Cards
            .Where(card => card.Keywords.Contains(CardKeyword.Ethereal))
            .ToList();

        foreach (var card in voidCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .TargetingAllOpponents(combatState)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}


