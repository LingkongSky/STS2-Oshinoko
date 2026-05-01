using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

// 每次打出消耗了谋划的牌时，抽1张牌。
public class ProfilePower : OshinogoCustomPower
{
    private readonly Dictionary<CardModel, int> _planSnapshotBeforePlay = new();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return Task.CompletedTask;
        }

        if (!_planSnapshotBeforePlay.ContainsKey(cardPlay.Card))
        {
            _planSnapshotBeforePlay[cardPlay.Card] = Owner.GetPowerAmount<PlanPower>();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return;
        }

        if (!_planSnapshotBeforePlay.TryGetValue(cardPlay.Card, out var before))
        {
            return;
        }

        _planSnapshotBeforePlay.Remove(cardPlay.Card);
        var after = Owner.GetPowerAmount<PlanPower>();
        if (before <= after)
        {
            return;
        }

        if (Owner.Player == null)
        {
            return;
        }

        await CardPileCmd.Draw(context, 1, Owner.Player);
    }
}
