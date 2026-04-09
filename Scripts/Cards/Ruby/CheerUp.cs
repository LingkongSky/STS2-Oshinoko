using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得1点费用。

[Pool(typeof(RubyCardPool))]
public class CheerUp : OshiCardModel
{
    private const string CalculatedEnergyKey = "CalculatedEnergy";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
     new CardsVar(1),
        new CalculationExtraVar(1m),
   ShineScaling.CreateCalculatedVar(CalculatedEnergyKey, ShineValueType.Cards)
        ];


    public CheerUp() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var finalEnergy = ShineScaling.Calculate(DynamicVars, CalculatedEnergyKey, cardPlay.Target);

        await PlayerCmd.GainEnergy(finalEnergy, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
