namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
public class Profile : AquaCardModel
{
    public Profile() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ProfilePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
