using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

// 永久复仇值：长期存在，并在打出闪耀牌时触发失去生命。
public class RevengePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/ui/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await RevengePowerHelper.TriggerShineCardCost(context, this, cardPlay);
    }
}

// 回合复仇值：本回合生效，回合结束后移除。
public class TurnRevengePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/ui/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await RevengePowerHelper.TriggerShineCardCost(context, this, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// 临时复仇值：下一次打出闪耀牌后触发并移除。
public class TempRevengePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/ui/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        await RevengePowerHelper.TriggerShineCardCost(context, this, cardPlay);
        await PowerCmd.Remove(this);
    }
}

// 隐藏返还层：用于回合结束返还被“临时/回合闪耀”过度抵消的永久复仇。
public class RevengeRefundPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldPlayVfx => false;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        if (Amount > 0)
        {
            await PowerCmd.Apply<RevengePower>(Owner, Amount, Owner, null, silent: true);
        }

        await PowerCmd.Remove(this);
    }
}

// 复仇值的统一规则入口。
public static class RevengePowerHelper
{
    // 总复仇 = 永久 + 回合 + 临时。
    public static int GetTotalRevenge(Creature creature)
    {
        return creature.GetPowerAmount<RevengePower>()
             + creature.GetPowerAmount<TurnRevengePower>()
             + creature.GetPowerAmount<TempRevengePower>();
    }

    // 获得复仇时按照“临时 -> 回合 -> 永久”顺序抵消闪耀值。
    // 若临时/回合复仇抵消了永久闪耀，会在回合结束后返还。
    public static async Task ApplyRevenge(Creature target, decimal amount, ValueDuration duration, Creature? applier, CardModel? cardSource)
    {
        var remaining = (int)amount;
        if (remaining <= 0)
        {
            return;
        }

        if (!target.HasPower<FusionPower>())
        {
            var borrowedPermanentShine = 0;

            remaining -= await PowerOffsetUtility.ConsumePowerAmount<TempShinePower>(target, remaining, applier, cardSource);
            remaining -= await PowerOffsetUtility.ConsumePowerAmount<TurnShinePower>(target, remaining, applier, cardSource);

            var consumedPermanentShine = await PowerOffsetUtility.ConsumePowerAmount<ShinePower>(target, remaining, applier, cardSource);
            remaining -= consumedPermanentShine;

            if (duration != ValueDuration.Permanent)
            {
                borrowedPermanentShine += consumedPermanentShine;
            }

            if (borrowedPermanentShine > 0)
            {
                await PowerCmd.Apply<ShineRefundPower>(target, borrowedPermanentShine, applier, cardSource, silent: true);
            }
        }

        if (remaining <= 0)
        {
            return;
        }

        switch (duration)
        {
            case ValueDuration.Permanent:
                await PowerCmd.Apply<RevengePower>(target, remaining, applier, cardSource);
                break;
            case ValueDuration.Turn:
                await PowerCmd.Apply<TurnRevengePower>(target, remaining, applier, cardSource);
                break;
            case ValueDuration.Temp:
                await PowerCmd.Apply<TempRevengePower>(target, remaining, applier, cardSource);
                break;
        }
    }

    // 仅让一种复仇来源触发扣血，避免永久/回合/临时并存时重复触发。
    public static bool ShouldHandleLoss(PowerModel power)
    {
        if (power.Owner.HasPower<RevengePower>())
        {
            return power is RevengePower;
        }

        if (power.Owner.HasPower<TurnRevengePower>())
        {
            return power is TurnRevengePower;
        }

        return power is TempRevengePower;
    }

    // 打出带闪耀关键词的牌时，失去等同于当前总复仇值的生命。
    public static async Task TriggerShineCardCost(PlayerChoiceContext context, PowerModel power, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != power.Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        if (!ShouldHandleLoss(power))
        {
            return;
        }

        var revenge = GetTotalRevenge(power.Owner);
        if (revenge <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(
            context,
            power.Owner,
            revenge,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            power.Owner
        );
    }
}
