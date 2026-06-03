using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ʧȥ2�����������20��񵲡������غ���ʧȥ����������3���ơ�

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



