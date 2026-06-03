using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Oshinoko.Scripts.Powers;
using System.Runtime.CompilerServices;

namespace Oshinoko.Scripts.Cards.Other;

public static class PlanCostHelper
{
    private sealed class PlanPlayMarker
    {
        public bool ConsumedPlanThisPlay;
    }

    private static readonly ConditionalWeakTable<CardModel, PlanPlayMarker> PlanPlayMarkers = new();

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
        MarkPlanConsumed(source);
        return true;
    }

    public static void MarkPlanConsumed(CardModel card)
    {
        PlanPlayMarkers.GetOrCreateValue(card).ConsumedPlanThisPlay = true;
    }

    public static bool ConsumePlanConsumedMark(CardModel card)
    {
        if (!PlanPlayMarkers.TryGetValue(card, out var marker) || !marker.ConsumedPlanThisPlay)
        {
            return false;
        }

        marker.ConsumedPlanThisPlay = false;
        return true;
    }
}
