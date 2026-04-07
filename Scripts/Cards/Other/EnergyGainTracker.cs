using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Oshinogo.Scripts.Cards.Other;

public static class EnergyGainTracker
{
    private sealed class State
    {
        public int LastRound;
        public CombatSide LastSide;
        public int LastEnergy;
        public bool GainedThisTurn;
        public bool IsSubscribed;
    }

    private static readonly Dictionary<Player, State> States = new();

    public static bool GainedEnergyThisTurn(Player player)
    {
        EnsureTracking(player);
        if (!States.TryGetValue(player, out var state))
        {
            return false;
        }

        var combatState = player.Creature.CombatState;
        if (combatState != null)
        {
            SyncTurnState(player, state, combatState);
        }

        return state.GainedThisTurn;
    }

    private static void EnsureTracking(Player player)
    {
        if (!States.TryGetValue(player, out var state))
        {
            state = new State();
            States[player] = state;
        }

        if (!state.IsSubscribed)
        {
            player.PlayerCombatState.EnergyChanged += (_, newEnergy) =>
            {
                var combatState = player.Creature.CombatState;
                if (combatState == null)
                {
                    return;
                }

                SyncTurnState(player, state, combatState);
                if (newEnergy > state.LastEnergy)
                {
                    state.GainedThisTurn = true;
                }

                state.LastEnergy = newEnergy;
            };
            state.IsSubscribed = true;
        }

        var currentCombat = player.Creature.CombatState;
        if (currentCombat != null)
        {
            SyncTurnState(player, state, currentCombat);
            state.LastEnergy = player.PlayerCombatState.Energy;
        }
    }

    private static void SyncTurnState(Player player, State state, CombatState combatState)
    {
        if (state.LastRound == combatState.RoundNumber && state.LastSide == combatState.CurrentSide)
        {
            return;
        }

        state.LastRound = combatState.RoundNumber;
        state.LastSide = combatState.CurrentSide;
        state.GainedThisTurn = false;
        state.LastEnergy = player.PlayerCombatState.Energy;
    }
}
