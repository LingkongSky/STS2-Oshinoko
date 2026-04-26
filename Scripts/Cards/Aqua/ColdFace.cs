using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 给予所有敌人2(3)层易伤。 谋划1
public class ColdFace : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Vulnerable", 2)];
    protected override IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> ExtraHoverTips => PlanCostHelper.CreatePlanCostHoverTips(1);

    public ColdFace() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 1))
        {
            return;
        }

        var enemies = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Vulnerable"].UpgradeValueBy(1);
    }
}
