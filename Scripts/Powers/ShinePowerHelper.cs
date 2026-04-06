using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

// 数值持续类型：永久、回合、临时。
public enum ValueDuration
{
    Permanent,
    Turn,
    Temp,
}

// 闪耀值的统一规则入口。
public static class ShinePowerHelper
{
    // 总闪耀 = 永久 + 回合 + 临时。
    public static int GetTotalShine(Creature creature)
    {
        return creature.GetPowerAmount<ShinePower>()
             + creature.GetPowerAmount<TurnShinePower>()
             + creature.GetPowerAmount<TempShinePower>();
    }

    // 获得闪耀时按照“临时 -> 回合 -> 永久”顺序抵消复仇值。
    // 若临时/回合闪耀抵消了永久复仇，会在回合结束后返还。
    public static async Task ApplyShine(Creature target, decimal amount, ValueDuration duration, Creature? applier, CardModel? cardSource)
    {
        var remaining = (int)amount;
        if (remaining <= 0)
        {
            return;
        }

        if (!target.HasPower<FusionPower>())
        {
            var borrowedPermanentRevenge = 0;

            remaining -= await PowerOffsetUtility.ConsumePowerAmount<TempRevengePower>(target, remaining, applier, cardSource);
            remaining -= await PowerOffsetUtility.ConsumePowerAmount<TurnRevengePower>(target, remaining, applier, cardSource);

            var consumedPermanentRevenge = await PowerOffsetUtility.ConsumePowerAmount<RevengePower>(target, remaining, applier, cardSource);
            remaining -= consumedPermanentRevenge;

            if (duration != ValueDuration.Permanent)
            {
                borrowedPermanentRevenge += consumedPermanentRevenge;
            }

            if (borrowedPermanentRevenge > 0)
            {
                await PowerCmd.Apply<RevengeRefundPower>(target, borrowedPermanentRevenge, applier, cardSource, silent: true);
            }
        }

        if (remaining <= 0)
        {
            return;
        }

        switch (duration)
        {
            case ValueDuration.Permanent:
                await PowerCmd.Apply<ShinePower>(target, remaining, applier, cardSource);
                break;
            case ValueDuration.Turn:
                await PowerCmd.Apply<TurnShinePower>(target, remaining, applier, cardSource);
                break;
            case ValueDuration.Temp:
                await PowerCmd.Apply<TempShinePower>(target, remaining, applier, cardSource);
                break;
        }
    }
}

// 抵消层数的通用工具，避免闪耀/复仇规则重复实现。
internal static class PowerOffsetUtility
{
    public static async Task<int> ConsumePowerAmount<T>(Creature target, int amount, Creature? applier, CardModel? cardSource) where T : PowerModel
    {
        if (amount <= 0)
        {
            return 0;
        }

        var power = target.GetPower<T>();
        if (power == null)
        {
            return 0;
        }

        var consumed = Math.Min(amount, power.Amount);
        if (consumed > 0)
        {
            await PowerCmd.ModifyAmount(power, -consumed, applier, cardSource, silent: true);
        }

        return consumed;
    }
}
