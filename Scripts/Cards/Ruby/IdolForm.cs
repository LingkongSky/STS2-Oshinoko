using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
public class IdolForm : OshiCardModel
{
    private const string ThresholdKey = "Threshold";

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ShineDymicVar(2m),
        new DynamicVar(ThresholdKey, 5),
        ];

    public IdolForm() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, DynamicVars[ShineDymicVar.Key].BaseValue, ValueDuration.Temp, Owner.Creature, this);
        if (ShinePowerHelper.GetTotalShine(Owner.Creature) > DynamicVars[ThresholdKey].BaseValue)
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars[ThresholdKey].UpgradeValueBy(-1);
    }

}
