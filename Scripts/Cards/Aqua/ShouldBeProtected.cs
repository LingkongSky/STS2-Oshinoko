using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 给予所有敌人2(3)层虚弱，2(3)层易伤，2(3)层摧残。
public class ShouldBeProtected : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Weak", 2),
        new DynamicVar("Vulnerable", 2),
        new DynamicVar("Debilitate", 2),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public ShouldBeProtected() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<WeakPower>(enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<DebilitatePower>(enemy, DynamicVars["Debilitate"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1);
        DynamicVars["Vulnerable"].UpgradeValueBy(1);
        DynamicVars["Debilitate"].UpgradeValueBy(1);
    }
}

