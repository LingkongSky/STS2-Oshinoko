namespace Oshinogo.Scripts.Powers;


/// 每当消耗卡牌时，获得临时复仇。
public class NightmaresPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (Owner.Player == null || card.Owner != Owner.Player)
        {
            return;
        }

        await RevengePowerHelper.ApplyRevenge(Owner, Amount, ValueDuration.Temp, Owner, null);
    }
}
