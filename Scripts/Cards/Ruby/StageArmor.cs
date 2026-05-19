using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 每失去1点生命，获得2(3)点格挡。

[RegisterCard(typeof(RubyCardPool))]
public class StageArmor : RubyCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockPerHp", 2)];

    public StageArmor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StageArmorPower>(choiceContext, Owner.Creature, DynamicVars["BlockPerHp"].BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerHp"].UpgradeValueBy(1);
    }
}


