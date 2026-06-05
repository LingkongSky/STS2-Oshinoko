using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// 描述: 场上每有一名敌人，额外获得5(7)点格挡。

[RegisterCard(typeof(RubyCardPool))]
public class IdolDisguise : RubyCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword()];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public IdolDisguise() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        var count = combatState?.Enemies.Count(e => e.IsAlive) ?? 0;
        var perEnemyBlock = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, Owner.Creature);
        var totalBlock = perEnemyBlock * count;
        await CreatureCmd.GainBlock(Owner.Creature, totalBlock, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}



