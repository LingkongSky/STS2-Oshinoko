using STS2RitsuLib.Keywords;

namespace Oshinoko.Scripts.Cards.Ruby;

// 描述: 获得12(16)点格挡，在卡组里增加1张眩晕。

[RegisterCard(typeof(RubyCardPool))]
public class FirstAppearance : RubyCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinokoKeywords.Shine.GetModKeywordCardKeyword(), CardKeyword.Exhaust];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public FirstAppearance() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner == null)
        {
            // Owner not set; skip.
            return;
        }

        if (owner.Creature == null)
        {
            // No creature bound yet; skip.
            return;
        }

        var block = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, cardPlay.Target);
        await CreatureCmd.GainBlock(owner.Creature, block, ValueProp.Move, cardPlay);
        if (CardScope == null)
        {
            // No card scope available to create status card.
            return;
        }

        var stun = CardScope.CreateCard<Stunned>(owner!);
        if (stun != null)
        {
            await CardPileCmd.Add(stun, PileType.Draw);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}


