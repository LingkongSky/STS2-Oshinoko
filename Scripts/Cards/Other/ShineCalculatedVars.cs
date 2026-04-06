using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Other;

// 标记“闪耀加成”应作用到哪一种基础动态数值。
public enum ShineValueType
{
    Damage,
    Block,
    Cards,
    Energy,
}

// 通用的“基础值 + 闪耀值”计算变量，可用于伤害/格挡/抽牌/能量等数值展示。
public class ShineCalculatedDamageVar : CalculatedVar
{
    private readonly ShineValueType _valueType;

    public ShineCalculatedDamageVar(string name, ShineValueType valueType) : base(name)
    {
        _valueType = valueType;
    }

    protected override DynamicVar GetBaseVar()
    {
        var card = (CardModel)_owner!;
        // 按类型绑定对应的基础动态变量，最终值由 CalculatedVar 统一计算。
        return _valueType switch
        {
            ShineValueType.Damage => card.DynamicVars.Damage,
            ShineValueType.Block => card.DynamicVars.Block,
            ShineValueType.Cards => card.DynamicVars.Cards,
            ShineValueType.Energy => card.DynamicVars.Energy,
            _ => card.DynamicVars.Damage,
        };
    }
}

// 专用于闪耀加成的伤害变量，改写基础值与额外值来源，避免依赖默认 CalculationBase/ExtraDamage 键。
public class ShineCalculatedDamageDisplayVar : CalculatedDamageVar
{
    public ShineCalculatedDamageDisplayVar(ValueProp props) : base(props)
    {
    }

    protected override DynamicVar GetBaseVar()
    {
        return ((CardModel)_owner!).DynamicVars.Damage;
    }

    protected override DynamicVar GetExtraVar()
    {
        return ((CardModel)_owner!).DynamicVars.CalculationExtra;
    }
}

// 闪耀加成卡牌的复用工具：创建变量与读取计算值都统一走这里。
public static class ShineScaling
{
    // 创建一个“基础值 + 闪耀值”的伤害变量，走引擎内置 CalculatedDamage 通道，
    // 以便卡面攻击数值和实际结算保持一致。
    public static CalculatedDamageVar CreateCalculatedDamageVar(ValueProp props)
    {
        var calculatedDamageVar = new ShineCalculatedDamageDisplayVar(props);
        calculatedDamageVar.WithMultiplier(GetShineMultiplier);
        return calculatedDamageVar;
    }

    // 创建一个“基础值 + 闪耀值”的计算变量，自动绑定闪耀倍率计算函数。
    public static CalculatedVar CreateCalculatedVar(string name, ShineValueType valueType)
    {
        return new ShineCalculatedDamageVar(name, valueType).WithMultiplier(GetShineMultiplier);
    }

    // 读取指定 key 的计算结果，便于在 OnPlay 中复用。
    public static decimal Calculate(DynamicVarSet dynamicVars, string key, Creature? target)
    {
        return ((CalculatedVar)dynamicVars[key]).Calculate(target);
    }

    // 闪耀倍率：当前角色的总闪耀值（永久 + 回合 + 临时）。
    private static decimal GetShineMultiplier(CardModel card, Creature? _)
    {
        return ShinePowerHelper.GetTotalShine(card.Owner.Creature);
    }
}
