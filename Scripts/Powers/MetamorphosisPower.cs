using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Oshinoko.Scripts.Powers;


/// 不再受到虚弱、脆弱、易伤影响�?
public class MetamorphosisPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        if (target == Owner && (canonicalPower is WeakPower || canonicalPower is FrailPower || canonicalPower is VulnerablePower))
        {
            modifiedAmount = 0m;
            return true;
        }

        return base.TryModifyPowerAmountReceived(canonicalPower, target, amount, applier, out modifiedAmount);
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var weak = Owner.GetPower<WeakPower>();
        if (weak != null)
        {
            await PowerCmd.Remove(weak);
        }

        var frail = Owner.GetPower<FrailPower>();
        if (frail != null)
        {
            await PowerCmd.Remove(frail);
        }

        var vulnerable = Owner.GetPower<VulnerablePower>();
        if (vulnerable != null)
        {
            await PowerCmd.Remove(vulnerable);
        }
    }
}
