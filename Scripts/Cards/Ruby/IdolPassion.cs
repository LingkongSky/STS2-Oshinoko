using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得2(3)点回合闪耀值。

[Pool(typeof(RubyCardPool))]
public class IdolPassion : OshiCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(1m)];

    public IdolPassion() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Turn, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ShineDymicVar.Key].UpgradeValueBy(1);
    }
}
