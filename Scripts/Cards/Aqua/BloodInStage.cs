namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
[RegisterCharacterStarterCard(typeof(Character.Aqua), 1)]

// 描述: 获得1(2)张浸血花瓣。
public class BloodInStage : AquaCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<BloodFlower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public BloodInStage() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var bloodFlower = Owner.Creature.CombatState?.CreateCard<BloodFlower>(Owner);
            if (bloodFlower != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(bloodFlower, PileType.Hand, Owner, CardPilePosition.Top);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}



