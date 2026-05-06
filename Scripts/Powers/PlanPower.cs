using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinogo.Scripts.Powers;


/// 谋划层数。当前先作为可叠加计数能力使用，供阿库娅卡牌与能力联动。
public class PlanPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
