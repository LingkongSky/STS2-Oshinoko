using MegaCrit.Sts2.Core.Hooks;

namespace Oshinoko.Scripts.Cards.Other;

public enum ShineValueType
{
    Damage,
    Block,
    Cards,
    Energy,
}

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

public class ShineCalculatedBlockVar : ShineCalculatedDamageVar
{
    public ShineCalculatedBlockVar(string name) : base(name, ShineValueType.Block)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        decimal value = Calculate(target);
        if (runGlobalHooks && card.CombatState != null && card.Owner?.Creature != null)
        {
            var blockVar = card.DynamicVars.Block as BlockVar;
            var props = blockVar?.Props ?? ValueProp.Move;
            value = Hook.ModifyBlock(card.CombatState, card.Owner.Creature, value, props, card, null, out _);
        }

        PreviewValue = value;
    }
}

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

public static class ShineScaling
{
    // 鍒涘缓浼ゅ鍙橀噺锛岀‘淇濆崱闈㈡樉绀哄€煎拰鎴樻枟缁撶畻鍊间竴鑷淬€?
    public static CalculatedDamageVar CreateCalculatedDamageVar(ValueProp props)
    {
        var calculatedDamageVar = new ShineCalculatedDamageDisplayVar(props);
        calculatedDamageVar.WithMultiplier((card, _) => GetCombinedMultiplier(card, ShineValueType.Damage));
        return calculatedDamageVar;
    }

    public static CalculatedVar CreateCalculatedVar(string name, ShineValueType valueType)
    {
        var calculatedVar = valueType == ShineValueType.Block
            ? new ShineCalculatedBlockVar(name)
            : new ShineCalculatedDamageVar(name, valueType);

        return calculatedVar.WithMultiplier((card, _) => GetCombinedMultiplier(card, valueType));
    }


    public static int GetShineUsedByCard(CardModel card)
    {
        if (card.Owner?.Creature == null)
        {
            return 0;
        }

        if (!card.Keywords.Contains(OshinokoKeywords.Shine.GetModKeywordCardKeyword()))
        {
            return 0;
        }

        if (!card.DynamicVars.ContainsKey("CalculationExtra"))
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

        if (!card.Keywords.Contains(OshinokoKeywords.Shine.GetModKeywordCardKeyword()))
        {
            return 0;
        }

        if (!card.DynamicVars.ContainsKey("CalculationExtra"))
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
