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
        return HasDrewCardThisTurn(player, includeHandDraw: true);
    }

    public static bool HasDrewCardThisTurn(Player? player, bool includeHandDraw)
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
            .Any(entry => entry.Actor == player.Creature
                && entry.HappenedThisTurn(combatState)
                && (includeHandDraw || !entry.FromHandDraw));
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

    public static bool HasPlayedShineCardWithShineThisTurn(Player? player)
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

        var currentShine = ShinePowerHelper.GetTotalShine(player.Creature);
        var netShineDeltaThisTurn = CombatManager.Instance.History.Entries
            .OfType<PowerReceivedEntry>()
            .Where(entry => entry.Actor == player.Creature
                && entry.HappenedThisTurn(combatState)
                && entry.Power is ShinePower or TurnShinePower or TempShinePower)
            .Sum(entry => entry.Amount);

        var runningShine = currentShine - netShineDeltaThisTurn;

        foreach (var entry in CombatManager.Instance.History.Entries)
        {
            if (entry.Actor != player.Creature || !entry.HappenedThisTurn(combatState))
            {
                continue;
            }

            if (entry is PowerReceivedEntry powerEntry
                && powerEntry.Power is ShinePower or TurnShinePower or TempShinePower)
            {
                runningShine += powerEntry.Amount;
                continue;
            }

            if (entry is CardPlayFinishedEntry playEntry
                && playEntry.CardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine)
                && runningShine > 0)
            {
                return true;
            }
        }

        return false;
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
