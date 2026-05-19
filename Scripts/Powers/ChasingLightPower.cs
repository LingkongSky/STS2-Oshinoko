namespace Oshinogo.Scripts.Powers;

public class ChasingLightPower : OshinogoCustomPower
{
    private class Data
    {
        public int ShineSpent;
        public int TriggerCount;
    }

    private const int DefaultThreshold = 6;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override int DisplayAmount => ResolveThreshold() - GetInternalData<Data>().ShineSpent % ResolveThreshold();

    protected override object InitInternalData() => new Data();

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
        {
            return;
        }

        var usedShine = Cards.Other.ShineScaling.GetShineUsedByCard(cardPlay.Card);
        if (usedShine <= 0)
        {
            return;
        }

        var threshold = ResolveThreshold();
        var data = GetInternalData<Data>();
        data.ShineSpent += usedShine;
        var totalTriggers = data.ShineSpent / threshold - data.TriggerCount;

        if (totalTriggers <= 0 || Owner.Player == null)
        {
            InvokeDisplayAmountChanged();
            return;
        }

        Flash();
        await PlayerCmd.GainEnergy(totalTriggers, Owner.Player);
        data.TriggerCount += totalTriggers;
        InvokeDisplayAmountChanged();
    }

    private int ResolveThreshold()
    {
        return Math.Max(1, Amount > 0 ? Amount : DefaultThreshold);
    }
}
