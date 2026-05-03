using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 获得2点闪耀，获得3点回合闪耀。
public class CapUnderMoon : AquaCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    private const string TurnShineKey = "TurnShine";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(2m), new DynamicVar(TurnShineKey, 3m)];

    public CapUnderMoon() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Permanent, Owner.Creature, this);
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[TurnShineKey].BaseValue, ValueDuration.Turn, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
