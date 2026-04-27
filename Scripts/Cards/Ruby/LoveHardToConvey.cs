using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎶藉彇绛夊悓浜庨棯鑰€鏁伴噺鐨勫崱鐗屻€傛娊X寮犲崱銆?

[Pool(typeof(RubyCardPool))]
public class LoveHardToConvey : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
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
