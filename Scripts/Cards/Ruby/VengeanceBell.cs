using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 失去2(1)点生命，令所有敌人失去9点生命，下回合开始时令所有敌人失去9点生命。
[Pool(typeof(RubyCardPool))]
public class VengeanceBell : OshiCardModel
{
    private const string ThresholdKey = "Threshold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar(ThresholdKey, 2),
    ];

    public VengeanceBell() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars[ThresholdKey].BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature
        );

        var opponents = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        await CreatureCmd.Damage(choiceContext, opponents, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this);

        await PowerCmd.Apply<VengeanceBellNextTurnPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars[ThresholdKey].UpgradeValueBy(-1);

    }
}
