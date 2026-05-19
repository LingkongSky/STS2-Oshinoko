namespace Oshinogo.Scripts.Powers;

public class UnreachablePower : OshinogoCustomPower
{
    private class Data
    {
        public int drawCount;
        public int triggerCount;
    }

    private const int DrawIncrement = 7;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override int DisplayAmount => DrawIncrement - GetInternalData<Data>().drawCount % DrawIncrement;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (Owner.Player == null || card.Owner != Owner.Player)
        {
            return;
        }

        var data = GetInternalData<Data>();
        data.drawCount += 1;
        var triggers = data.drawCount / DrawIncrement - data.triggerCount;
        if (triggers <= 0)
        {
            InvokeDisplayAmountChanged();
            return;
        }

        // Update first to avoid re-entrant extra draws counting as still-unclaimed triggers.
        data.triggerCount += triggers;
        Flash();
        await CardPileCmd.Draw(choiceContext, triggers, Owner.Player);
        InvokeDisplayAmountChanged();
    }
}
