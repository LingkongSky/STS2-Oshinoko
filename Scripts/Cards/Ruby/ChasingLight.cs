using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 每使用6(5)点闪耀值，获得1点能量。

[Pool(typeof(RubyCardPool))]
public class ChasingLight : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    private const string ThresholdKey = "Threshold";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(ThresholdKey, 6)];

    public ChasingLight() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ChasingLightPower>(Owner.Creature, DynamicVars[ThresholdKey].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ThresholdKey].UpgradeValueBy(-1);
    }
}
