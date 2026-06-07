using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

// 冷脸：给予所有敌人1(2)层易伤

[RegisterCard(typeof(AquaCardPool))]
public class ColdFace : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Vulnerable", 1)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("VULNERABLE");

    public ColdFace() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Vulnerable"].UpgradeValueBy(1);
    }
}

