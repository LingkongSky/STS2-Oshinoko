using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 失去2点生命，获得20点格挡。若本回合你失去过生命，抽3张牌。

[RegisterCard(typeof(RubyCardPool))]
public class SwitchToVengeance : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public SwitchToVengeance() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    new BlockVar(20m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        if (CombatHistoryHelper.HasLostHpThisTurn(Owner))
        {
            await CardPileCmd.Draw(choiceContext, 3, Owner);
        }

        await CreatureCmd.Damage(
        choiceContext,
        Owner.Creature,
        2,
        ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
        Owner.Creature
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



