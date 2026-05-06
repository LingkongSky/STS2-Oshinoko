using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Other;

public static class PlanCostHelper
{
    public static IEnumerable<IHoverTip> CreatePlanCostHoverTips(int amount)
    {
        var amountVar = new DynamicVar("Amount", amount);
        var title = new LocString("static_hover_tips", "PLAN_COST.title");
        var description = new LocString("static_hover_tips", "PLAN_COST.description");
        title.Add(amountVar);
        description.Add(amountVar);
        return [new HoverTip(title, description)];
    }

    public static bool HasEnoughPlan(Player? owner, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        return owner?.Creature != null && owner.Creature.GetPowerAmount<PlanPower>() >= amount;
    }

    public static async Task<bool> TryConsumePlan(Player? owner, CardModel source, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        var creature = owner?.Creature;
        if (creature == null)
        {
            return false;
        }

        var plan = creature.GetPower<PlanPower>();
        if (plan == null || plan.Amount < amount)
        {
            return false;
        }

        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), plan, -amount, creature, source, true);
        return true;
    }
}
