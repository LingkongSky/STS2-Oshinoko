namespace Oshinogo.Scripts.Powers;


/// 陷阱：目标受到的攻击伤害提高100%，并在目标回合结束时减少1层。
public class TrapPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }

        return 2m;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.TickDownDuration(this);
    }
}
