using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

// 永久闪耀值。
public class ShinePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/ui/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";
}

// 回合闪耀值：回合结束后移除。
public class TurnShinePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://Oshinogo/images/ui/ruby_energy.png";

    public override string? CustomBigIconPath => "res://Oshinogo/images/ui/ruby_energy_big.png";

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// 临时闪耀值：下一次打出一张带“闪耀”关键词的牌后移除。
public class TempShinePower : CustomPowerModel
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

        // 如果临时闪耀是由“当前这张牌”赋予的，那么跳过这一次，
        // 避免出现“刚获得就立刻被消耗”。
        if (TempPowerSourceTracker.ShouldSkipTempShine(Owner, cardPlay.Card))
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return;
        }

        await PowerCmd.Remove(this);
    }

    // 回合结束时清空所有临时闪耀层数
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            TempPowerSourceTracker.Clear(Owner);
            await PowerCmd.Remove(this);
        }
    }
}