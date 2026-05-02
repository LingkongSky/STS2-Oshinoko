using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 获得2(3)层脱身。 谋划1
public class Watch : AquaCardModel
{
    private const string EscapeKey = "WatchEscape";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(EscapeKey, 2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => PlanAndKeywordTips(1, "ESCAPE");
    public Watch() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 1))
        {
            return;
        }

        await PowerCmd.Apply<EscapePower>(Owner.Creature, DynamicVars[EscapeKey].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[EscapeKey].UpgradeValueBy(1);
    }
}
