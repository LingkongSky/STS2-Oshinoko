using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Oshinogo.Scripts.Powers;

// 统一计算角色当前可用于加成的总闪耀值。
public static class ShinePowerHelper
{
    // 总闪耀 = 永久闪耀 + 回合闪耀 + 临时闪耀。
    public static int GetTotalShine(Creature creature)
    {
        return creature.GetPowerAmount<ShinePower>()
             + creature.GetPowerAmount<TurnShinePower>()
             + creature.GetPowerAmount<TempShinePower>();
    }
}
