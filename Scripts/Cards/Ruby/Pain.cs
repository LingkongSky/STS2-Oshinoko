using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 所有敌人失去9(13)点生命值。

[RegisterCard(typeof(RubyCardPool))]
public class Pain : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move)];

    public Pain() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var opponents = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        await CreatureCmd.Damage(choiceContext, opponents, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, null, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}




