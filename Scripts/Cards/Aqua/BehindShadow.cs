using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 失去2点生命，获得2点回合复仇，翻倍所有敌人的负面效果层数。
public class BehindShadow : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public BehindShadow() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            2,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature
        );

        await RevengePowerHelper.ApplyRevenge(Owner.Creature, 2, ValueDuration.Turn, Owner.Creature, this);

        var opponents = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        foreach (var enemy in opponents)
        {
            var debuffs = enemy.Powers.Where(power => power.Type == PowerType.Debuff && power.Amount > 0).ToList();
            foreach (var debuff in debuffs)
            {
                await PowerCmd.ModifyAmount(choiceContext, debuff, debuff.Amount, Owner.Creature, this, true);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
