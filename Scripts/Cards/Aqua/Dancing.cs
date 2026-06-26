using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 获得7(10)点格挡，给予3(4)层流言
public class Dancing : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("RUMOR");

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7, ValueProp.Move), new DynamicVar("Rumor", 3)];

    public override bool GainsBlock => true;

    public Dancing() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<RumorPower>(choiceContext, cardPlay.Target, DynamicVars["Rumor"].BaseValue, Owner.Creature, this, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars["Rumor"].UpgradeValueBy(1);
    }
}



