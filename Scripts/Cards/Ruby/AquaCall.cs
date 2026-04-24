using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得1(2)点临时闪耀值。抽1(2)张牌。

[Pool(typeof(RubyCardPool))]
public class AquaCall : RubyCardModel
{
    private const string ThresholdKey = "Threshold";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ShineDymicVar(1m),
        new DynamicVar(ThresholdKey, 1)
    ];

    public AquaCall() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Temp, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars[ThresholdKey].BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ShineDymicVar.Key].UpgradeValueBy(1);
        DynamicVars[ThresholdKey].UpgradeValueBy(1);
    }
}
