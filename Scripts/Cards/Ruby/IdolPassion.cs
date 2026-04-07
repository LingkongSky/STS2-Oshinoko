using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
public class IdolPassion : OshiCardModel
{
    // 在本回合获得1(2)点回合闪耀值，消耗（保留）
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ShineDymicVar(2m)];

    public IdolPassion() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
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
