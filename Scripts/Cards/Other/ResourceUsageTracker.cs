using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinoko.Scripts.Cards.Other;

public static class ResourceUsageTracker
{
    private sealed class State
    {
        public ICombatState? CombatState;
        public int TotalShineGained;
        public int TotalShineSpent;
        public int TotalRevengeGained;
        public int TotalRevengeSpent;
        public int LastRound;
        public CombatSide LastSide;
        public bool FirstChangeTriggeredThisTurn;
        public bool RevengeGainTriggeredThisTurn;
    }

    private static readonly Dictionary<Player, State> States = new();

    public static void OnShineChanged(Player player, int delta)
    {
        var state = EnsureState(player);
        if (delta > 0)
        {
            state.TotalShineGained += delta;
        }
        else if (delta < 0)
        {
            state.TotalShineSpent += -delta;
        }
    }

    public static void OnRevengeChanged(Player player, int delta)
    {
        var state = EnsureState(player);
        if (delta > 0)
        {
            state.TotalRevengeGained += delta;
        }
        else if (delta < 0)
        {
            state.TotalRevengeSpent += -delta;
        }
    }

    public static int GetTotalShineGained(Player player)
    {
        return EnsureState(player).TotalShineGained;
    }

    public static int GetTotalShineSpent(Player player)
    {
        return EnsureState(player).TotalShineSpent;
    }

    public static int GetTotalRevengeGained(Player player)
    {
        return EnsureState(player).TotalRevengeGained;
    }

    public static int GetTotalRevengeSpent(Player player)
    {
        return EnsureState(player).TotalRevengeSpent;
    }



    private static State EnsureState(Player player)
    {
        if (!States.TryGetValue(player, out var state))
        {
            state = new State();
            States[player] = state;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            ResetState(state, null);
            return state;
        }

        if (state.CombatState != combatState)
        {
            ResetState(state, combatState);
            return state;
        }

        if (state.LastRound != combatState.RoundNumber || state.LastSide != combatState.CurrentSide)
        {
            state.LastRound = combatState.RoundNumber;
            state.LastSide = combatState.CurrentSide;
            state.FirstChangeTriggeredThisTurn = false;
            state.RevengeGainTriggeredThisTurn = false;
        }

        return state;
    }

    private static void ResetState(State state, ICombatState? combatState)
    {
        state.CombatState = combatState;
        state.TotalShineGained = 0;
        state.TotalShineSpent = 0;
        state.TotalRevengeGained = 0;
        state.TotalRevengeSpent = 0;
        state.FirstChangeTriggeredThisTurn = false;
        state.RevengeGainTriggeredThisTurn = false;
        state.LastRound = combatState?.RoundNumber ?? 0;
        state.LastSide = combatState?.CurrentSide ?? CombatSide.Player;
    }
}
