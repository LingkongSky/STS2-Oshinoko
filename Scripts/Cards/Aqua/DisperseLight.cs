
namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 对一个敌人造成9(14)点伤害，并使其失去2(3)点力量
public class DisperseLight : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new DynamicVar("StrengthLoss", 2),
    ];

    public DisperseLight() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars["StrengthLoss"].UpgradeValueBy(1);
    }
}




