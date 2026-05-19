using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;


/// 每次获得谋划时，额外获得1层。
public class BlackStarPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if (target != Owner || canonicalPower is not PlanPower || amount <= 0)
        {
            return base.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
        }

        modifiedAmount = amount + 1;
        return true;
    }
}
