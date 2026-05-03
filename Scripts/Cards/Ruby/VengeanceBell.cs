using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 失去1点生命，令所有敌人失去10点生命，下回合开始时再失去10点生命。获得2(3)点临时复仇。

[Pool(typeof(RubyCardPool))]
public class VengeanceBell : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("REVENGE");
    private const string SelfDamageKey = "SelfDamage";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar(SelfDamageKey, 1),
        new RevengeDynamicVar(2m),
    ];

    public VengeanceBell() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars[SelfDamageKey].BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature
        );

        var opponents = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        await CreatureCmd.Damage(choiceContext, opponents, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this);

        await RevengePowerHelper.ApplyRevenge(
            Owner.Creature,
            DynamicVars[RevengeDynamicVar.Key].BaseValue,
            ValueDuration.Temp,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<Powers.VengeanceBellPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[RevengeDynamicVar.Key].UpgradeValueBy(1);

    }
}
