using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

/// 每抽到指定数量的牌后，额外抽1张牌。
public class UnreachablePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    // Each entry is one independently-resolving copy of this effect.
    private List<int> _thresholds = [];
    private List<int> _drawCounters = [];

    public override int DisplayAmount
    {
        get
        {
            EnsureTrackingInitialized();
            if (_thresholds.Count == 0)
            {
                return 0;
            }

            var minRemaining = int.MaxValue;
            for (var i = 0; i < _thresholds.Count; i++)
            {
                var threshold = Math.Max(1, _thresholds[i]);
                var remaining = threshold - _drawCounters[i] % threshold;
                if (remaining < minRemaining)
                {
                    minRemaining = remaining;
                }
            }

            return minRemaining == int.MaxValue ? 0 : minRemaining;
        }
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || amount <= 0)
        {
            return Task.CompletedTask;
        }

        EnsureTrackingInitialized();
        _thresholds.Add(Math.Max(1, (int)amount));
        _drawCounters.Add(0);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (Owner.Player == null || card.Owner != Owner.Player)
        {
            return;
        }

        EnsureTrackingInitialized();

        var totalBonusDraw = 0;
        for (var i = 0; i < _thresholds.Count; i++)
        {
            var threshold = Math.Max(1, _thresholds[i]);
            var counter = _drawCounters[i] + 1;
            totalBonusDraw += counter / threshold;
            _drawCounters[i] = counter % threshold;
        }

        InvokeDisplayAmountChanged();

        if (totalBonusDraw <= 0)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, totalBonusDraw, Owner.Player);
    }

    private void EnsureTrackingInitialized()
    {
        if (_thresholds.Count > 0)
        {
            if (_drawCounters.Count == _thresholds.Count)
            {
                return;
            }

            while (_drawCounters.Count < _thresholds.Count)
            {
                _drawCounters.Add(0);
            }

            if (_drawCounters.Count > _thresholds.Count)
            {
                _drawCounters = _drawCounters.Take(_thresholds.Count).ToList();
            }

            return;
        }

        // Backward compatibility for existing saves.
        if (Amount > 0)
        {
            _thresholds.Add(Math.Max(1, Amount));
            _drawCounters.Add(0);
        }
    }
}
