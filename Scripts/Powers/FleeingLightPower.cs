using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;


public class FleeingLightPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _spent;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var usedRevenge = ShineScaling.GetRevengeUsedByCard(cardPlay.Card);
        if (usedRevenge <= 0)
        {
            return;
        }

        _spent += usedRevenge;
        InvokeDisplayAmountChanged();

        if (_spent < Amount)
        {
            return;
        }

        var triggers = (int)Math.Floor((decimal)_spent / Amount);
        _spent = _spent % triggers;


        if (Owner.Player == null)
        {
            return;
        }

        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), triggers, Owner.Player);
        InvokeDisplayAmountChanged();

    }

}
