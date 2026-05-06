using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
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
