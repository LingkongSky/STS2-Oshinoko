using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// ����: �������е���2(3)��������2(3)�����ˣ�2(3)��ݲС�
public class ShouldBeProtected : AquaCardModel
{
    private const string WeakKey = "ShouldBeProtectedWeak";
    private const string VulnerableKey = "ShouldBeProtectedVulnerable";
    private const string DebilitateKey = "ShouldBeProtectedDebilitate";

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("VULNERABLE", "WEAK", "DEBILITATE");
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




