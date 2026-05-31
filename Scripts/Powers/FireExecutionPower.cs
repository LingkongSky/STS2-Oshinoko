namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 对敌人造成未被格挡伤害时，给予流言。
/// </summary>
public class FireExecutionPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || result.UnblockedDamage <= 0)
        {
            return;
        }

        if (target.Side == Owner.Side || target.IsDead)
        {
            return;
        }

        await PowerCmd.Apply<RumorPower>(choiceContext, target, Amount, Owner, cardSource, true);
    }
}
