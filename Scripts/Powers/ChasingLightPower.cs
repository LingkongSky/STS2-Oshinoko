using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using Oshinogo.Scripts.Cards.Other;

namespace Oshinogo.Scripts.Powers;




public class ChasingLightPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _spent;

    public override int DisplayAmount => Amount - _spent % Amount;


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

        _spent += usedShine;
        InvokeDisplayAmountChanged();

        if (_spent < Amount)
        {
            return;
        }

        var triggers = (int)Math.Floor((decimal)_spent / Amount);
        _spent = _spent % triggers;

        /*
        var threshold = Math.Max(1, Amount);
        var triggers = _spent / threshold;
        if (triggers <= 0)
        {
            return;
        }

        _spent -= triggers * threshold;
        if (Owner.Player == null)
        {
            // No player to draw for.
            return;
        }
        */
        await PlayerCmd.GainEnergy(triggers, Owner.Player);
        InvokeDisplayAmountChanged();

    }

}