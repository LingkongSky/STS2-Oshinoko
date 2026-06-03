using MegaCrit.Sts2.Core.Entities.Powers;

namespace Oshinoko.Scripts.Powers;


/// ı����������ǰ����Ϊ�ɵ��Ӽ�������ʹ�ã�������櫿���������������
public class PlanPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
