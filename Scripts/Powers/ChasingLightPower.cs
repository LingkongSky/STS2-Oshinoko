using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;

public class ChasingLightPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Each entry represents one independently-resolving copy of this effect.
    private List<int> _thresholds = [];
    private List<int> _spentByInstance = [];

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
                var spent = _spentByInstance[i];
                var remaining = threshold - spent % threshold;
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
        _spentByInstance.Add(0);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var usedShine = ShineScaling.GetShineUsedByCard(cardPlay.Card);
        if (usedShine <= 0)
        {
            return;
        }

        EnsureTrackingInitialized();

        var totalTriggers = 0;
        for (var i = 0; i < _thresholds.Count; i++)
        {
            var threshold = Math.Max(1, _thresholds[i]);
            var spent = _spentByInstance[i] + usedShine;
            totalTriggers += spent / threshold;
            _spentByInstance[i] = spent % threshold;
        }

        InvokeDisplayAmountChanged();

        if (totalTriggers <= 0 || Owner.Player == null)
        {
            return;
        }

        await PlayerCmd.GainEnergy(totalTriggers, Owner.Player);
        InvokeDisplayAmountChanged();
    }

    private void EnsureTrackingInitialized()
    {
        if (_thresholds.Count > 0)
        {
            if (_spentByInstance.Count == _thresholds.Count)
            {
                return;
            }

            while (_spentByInstance.Count < _thresholds.Count)
            {
                _spentByInstance.Add(0);
            }

            if (_spentByInstance.Count > _thresholds.Count)
            {
                _spentByInstance = _spentByInstance.Take(_thresholds.Count).ToList();
            }

            return;
        }

        // Backward-compatibility: if loaded from an old save with only Amount, seed one instance.
        if (Amount > 0)
        {
            _thresholds.Add(Math.Max(1, Amount));
            _spentByInstance.Add(0);
        }
    }
}
