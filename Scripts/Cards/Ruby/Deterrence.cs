using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 对所有敌人造成5(7)点伤害，给予1(2)层易伤。

[Pool(typeof(RubyCardPool))]
public class Deterrence : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("VULNERABLE");
    private const string VulnerableKey = "Vulnerable";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar(VulnerableKey, 1),
    ];

    public Deterrence() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            // No combat state to target opponents.
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        var opponents = combatState.GetOpponentsOf(Owner.Creature);
        foreach (var enemy in opponents)
        {
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars[VulnerableKey].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars[VulnerableKey].UpgradeValueBy(1);
    }
}
