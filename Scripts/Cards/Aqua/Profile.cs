using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 获得3点闪耀，获得一层陷阱。
public class Profile : AquaCardModel
{
    public Profile() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, 3, ValueDuration.Permanent, Owner.Creature, this);

        await PowerCmd.Apply<TrapPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);

    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


