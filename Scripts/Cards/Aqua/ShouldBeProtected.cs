using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 给予所有敌人2(3)层虚弱，2(3)层易伤，2(3)层摧残。
public class ShouldBeProtected : AquaCardModel
{
    private const string WeakKey = "ShouldBeProtectedWeak";
    private const string VulnerableKey = "ShouldBeProtectedVulnerable";
    private const string DebilitateKey = "ShouldBeProtectedDebilitate";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("VULNERABLE", "WEAK", "DEBILITATE");
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(WeakKey, 2),
        new DynamicVar(VulnerableKey, 2),
        new DynamicVar(DebilitateKey, 2),
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
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars[WeakKey].BaseValue, Owner.Creature, this, true);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars[VulnerableKey].BaseValue, Owner.Creature, this, true);
            await PowerCmd.Apply<DebilitatePower>(choiceContext, enemy, DynamicVars[DebilitateKey].BaseValue, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[WeakKey].UpgradeValueBy(1);
        DynamicVars[VulnerableKey].UpgradeValueBy(1);
        DynamicVars[DebilitateKey].UpgradeValueBy(1);
    }
}

