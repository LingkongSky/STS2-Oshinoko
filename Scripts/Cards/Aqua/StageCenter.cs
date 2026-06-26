using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 获得2点闪耀，获得7层覆甲
public class StageCenter : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(2m), new DynamicVar("Plating", 7)];

    public StageCenter() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Permanent, Owner.Creature, this);
        await PowerCmd.Apply<PlatingPower>(choiceContext, Owner.Creature, DynamicVars["Plating"].BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


