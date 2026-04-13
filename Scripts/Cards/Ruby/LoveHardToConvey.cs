using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Ruby;

// 抽取等同于闪耀数量的卡牌。抽X张卡。

[Pool(typeof(RubyCardPool))]
public class LoveHardToConvey : OshiCardModel
{
    private const string CalculatedCardsKey = "CalculatedCards";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(0),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedCardsKey, ShineValueType.Cards),
    ];

    public LoveHardToConvey() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawCount = (int)ShineScaling.Calculate(DynamicVars, CalculatedCardsKey, cardPlay.Target);
        if (drawCount <= 0)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, drawCount, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
