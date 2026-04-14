using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Other;

// 标记“闪耀/复仇加成”作用到哪一种基础动态值。
public enum ShineValueType
{
    Damage,
    Block,
    Cards,
    Energy,
}

// 通用计算变量：基础值 +（闪耀与复仇规则）后的额外值。
// 适用于抽牌、能量、格挡等非伤害显示字段。
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

// 专用于伤害显示的变量，继承 CalculatedDamageVar 以走引擎伤害预览链路。
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

// 闪耀/复仇缩放复用入口：统一创建计算变量与读取计算结果。
public static class ShineScaling
{
    // 创建伤害变量，确保卡面显示值和战斗结算值一致。
    public static CalculatedDamageVar CreateCalculatedDamageVar(ValueProp props)
    {
        var calculatedDamageVar = new ShineCalculatedDamageDisplayVar(props);
        calculatedDamageVar.WithMultiplier((card, _) => GetCombinedMultiplier(card, ShineValueType.Damage));
        return calculatedDamageVar;
    }

    // 创建通用计算变量，用于抽牌/格挡/能量等字段。
    public static CalculatedVar CreateCalculatedVar(string name, ShineValueType valueType)
    {
        return new ShineCalculatedDamageVar(name, valueType).WithMultiplier((card, _) => GetCombinedMultiplier(card, valueType));
    }


    // 统一读取计算结果，避免卡牌里重复写类型转换。
    public static int GetShineUsedByCard(CardModel card)
    {
        if (card.Owner?.Creature == null)
        {
            return 0;
        }

        if (!card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return 0;
        }

        var extraVar = card.DynamicVars.CalculationExtra;
        if (extraVar == null || extraVar.BaseValue == 0)
        {
            return 0;
        }

        var shine = ShinePowerHelper.GetTotalShine(card.Owner.Creature);
        return Math.Max(0, shine);
    }

    public static int GetRevengeUsedByCard(CardModel card)
    {
        if (card.Owner?.Creature == null)
        {
            return 0;
        }

        if (!card.Keywords.Contains(OshinogoKeywords.Shine))
        {
            return 0;
        }

        var extraVar = card.DynamicVars.CalculationExtra;
        if (extraVar == null || extraVar.BaseValue == 0)
        {
            return 0;
        }

        var revenge = RevengePowerHelper.GetTotalRevenge(card.Owner.Creature);
        return Math.Max(0, revenge);
    }

    public static decimal Calculate(DynamicVarSet dynamicVars, string key, Creature? target)
    {
        return ((CalculatedVar)dynamicVars[key]).Calculate(target);
    }

    // 规则：先叠加闪耀，再按复仇乘算（无复仇按 1 倍）。
    private static decimal GetCombinedMultiplier(CardModel card, ShineValueType valueType)
    {
        var baseVar = GetBaseVarByType(card, valueType);
        var extraVar = card.DynamicVars.CalculationExtra;
        if (extraVar.BaseValue == 0)
        {
            return 0;
        }

        var shine = ShinePowerHelper.GetTotalShine(card.Owner.Creature);
        var revenge = RevengePowerHelper.GetTotalRevenge(card.Owner.Creature);
        var revengeMultiplier = revenge > 0 ? revenge : 1;

        var valueAfterShine = baseVar.BaseValue + extraVar.BaseValue * shine;
        var finalValue = valueAfterShine * revengeMultiplier;
        return (finalValue - baseVar.BaseValue) / extraVar.BaseValue;
    }

    private static DynamicVar GetBaseVarByType(CardModel card, ShineValueType valueType)
    {
        return valueType switch
        {
            ShineValueType.Damage => card.DynamicVars.Damage,
            ShineValueType.Block => card.DynamicVars.Block,
            ShineValueType.Cards => card.DynamicVars.Cards,
            ShineValueType.Energy => card.DynamicVars.Energy,
            _ => card.DynamicVars.Damage,
        };
    }
}
