namespace Oshinogo.Scripts.Powers;


/// 陷阱：目标受到的攻击伤害提高100%，并在怪物侧回合结束时减少1层。
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

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.TickDownDuration(this);
    }
}

