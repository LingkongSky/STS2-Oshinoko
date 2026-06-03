using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinoko.Scripts.Cards.Other;

namespace Oshinoko.Scripts.Powers;

public class FleeingLightPower : OshinokoCustomPower
{
    private class Data
    {
        public int RevengeSpent;
        public int TriggerCount;
    }

    private const int DefaultThreshold = 5;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override int DisplayAmount => ResolveThreshold() - GetInternalData<Data>().RevengeSpent % ResolveThreshold();

    protected override object InitInternalData() => new Data();

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

        var threshold = ResolveThreshold();
        var data = GetInternalData<Data>();
        data.RevengeSpent += usedRevenge;
        var triggers = data.RevengeSpent / threshold - data.TriggerCount;
        if (triggers <= 0 || Owner.Player == null)
        {
            InvokeDisplayAmountChanged();
            return;
        }

        Flash();
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), triggers, Owner.Player);
        data.TriggerCount += triggers;
        InvokeDisplayAmountChanged();
    }

    private int ResolveThreshold()
    {
        return Math.Max(1, Amount > 0 ? Amount : DefaultThreshold);
    }
}
