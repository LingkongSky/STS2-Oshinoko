using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Other;

public static class CombatHistoryHelper
{
    public static bool HasDrewCardThisTurn(Player? player)
    {
        if (player == null)
        {
            return false;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return false;
        }

        return CombatManager.Instance.History.Entries
            .OfType<CardDrawnEntry>()
            .Any(entry => entry.Actor == player.Creature && entry.HappenedThisTurn(combatState));
    }

    public static bool HasPlayedAttackThisTurn(Player? player)
    {
        if (player == null)
        {
            return false;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return false;
        }

        // Use finished plays to avoid counting cancelled plays.
        return CombatManager.Instance.History.Entries
            .OfType<CardPlayFinishedEntry>()
            .Any(entry => entry.Actor == player.Creature
                && entry.HappenedThisTurn(combatState)
                && entry.CardPlay.Card.Type == CardType.Attack);
    }

    public static bool HasGainedShineThisTurn(Player? player)
    {
        return HasPowerChangeThisTurn<ShinePower, TurnShinePower, TempShinePower>(player, gained: true);
    }

    public static bool HasGainedRevengeThisTurn(Player? player)
    {
        return HasPowerChangeThisTurn<RevengePower, TurnRevengePower, TempRevengePower>(player, gained: true);
    }

    public static bool HasSpentShineThisTurn(Player? player)
    {
        return HasPowerChangeThisTurn<ShinePower, TurnShinePower, TempShinePower>(player, gained: false);
    }

    public static bool HasSpentRevengeThisTurn(Player? player)
    {
        return HasPowerChangeThisTurn<RevengePower, TurnRevengePower, TempRevengePower>(player, gained: false);
    }

    public static bool HasLostHpThisTurn(Player? player)
    {
        if (player == null)
        {
            return false;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return false;
        }

        return CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Any(entry => entry.Receiver == player.Creature
                && entry.HappenedThisTurn(combatState)
                && entry.Result.UnblockedDamage > 0);
    }

    private static bool HasPowerChangeThisTurn<TPower, TTurnPower, TTempPower>(Player? player, bool gained)
        where TPower : PowerModel
        where TTurnPower : PowerModel
        where TTempPower : PowerModel
    {
        if (player == null)
        {
            return false;
        }

        var combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            return false;
        }

        return CombatManager.Instance.History.Entries
            .OfType<PowerReceivedEntry>()
            .Any(entry => entry.Actor == player.Creature
                && entry.HappenedThisTurn(combatState)
                && (gained ? entry.Amount > 0 : entry.Amount < 0)
                && entry.Power is TPower or TTurnPower or TTempPower);
    }
}
