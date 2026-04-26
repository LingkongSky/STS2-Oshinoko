using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 获得1(2)张浸血花瓣。
public class BloodInStage : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public BloodInStage() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CardScope == null)
        {
            return;
        }

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var bloodFlower = CardScope.CreateCard<BloodFlower>(Owner);
            if (bloodFlower != null)
            {
                await CardPileCmd.Add(bloodFlower, PileType.Hand);
            }
        }
    }

    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<Sinking>();

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
