namespace Oshinoko.Scripts.Cards.Ruby;

// 描述: 获得2(3)点回合闪耀。

[RegisterCard(typeof(RubyCardPool))]
[RegisterCharacterStarterCard(typeof(Character.Ruby), 1)]
public class IdolPassion : RubyCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(1m)];

    public IdolPassion() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Turn, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ShineDymicVar.Key].UpgradeValueBy(1);
    }
}



