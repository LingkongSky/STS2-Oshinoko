using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Other;

// 鏍囪鈥滈棯鑰€/澶嶄粐鍔犳垚鈥濅綔鐢ㄥ埌鍝竴绉嶅熀纭€鍔ㄦ€佸€笺€?
public enum ShineValueType
{
    Damage,
    Block,
    Cards,
    Energy,
}

// 閫氱敤璁＄畻鍙橀噺锛氬熀纭€鍊?+锛堥棯鑰€涓庡浠囪鍒欙級鍚庣殑棰濆鍊笺€?
// 閫傜敤浜庢娊鐗屻€佽兘閲忋€佹牸鎸＄瓑闈炰激瀹虫樉绀哄瓧娈点€?
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

// 涓撶敤浜庝激瀹虫樉绀虹殑鍙橀噺锛岀户鎵?CalculatedDamageVar 浠ヨ蛋寮曟搸浼ゅ棰勮閾捐矾銆?
// 闂傤亣鈧偓/婢跺秳绮愰弽cm灏呴崣姗€鍣洪敍姘▔缁€鍝勨偓闂寸瑐鐎电懓绨查崚鎷屽厴闁插繐顤冮惄濠佺缁粯鏅ラ弸婧库偓?
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

// 闂€€/澶嶄粐缂╂斁澶嶇敤鍏ュ彛锛氱粺涓€鍒涘缓璁＄畻鍙橀噺涓庤鍙栬绠楃粨鏋溿€?
public static class ShineScaling
{
    // 鍒涘缓浼ゅ鍙橀噺锛岀‘淇濆崱闈㈡樉绀哄€煎拰鎴樻枟缁撶畻鍊间竴鑷淬€?
    public static CalculatedDamageVar CreateCalculatedDamageVar(ValueProp props)
    {
        var calculatedDamageVar = new ShineCalculatedDamageDisplayVar(props);
        calculatedDamageVar.WithMultiplier((card, _) => GetCombinedMultiplier(card, ShineValueType.Damage));
        return calculatedDamageVar;
    }

    // 鍒涘缓閫氱敤璁＄畻鍙橀噺锛岀敤浜庢娊鐗?鏍兼尅/鑳介噺绛夊瓧娈点€?
    public static CalculatedVar CreateCalculatedVar(string name, ShineValueType valueType)
    {
        var calculatedVar = valueType == ShineValueType.Block
            ? new ShineCalculatedBlockVar(name)
            : new ShineCalculatedDamageVar(name, valueType);

        return calculatedVar.WithMultiplier((card, _) => GetCombinedMultiplier(card, valueType));
    }


    // 缁熶竴璇诲彇璁＄畻缁撴灉锛岄伩鍏嶅崱鐗岄噷閲嶅鍐欑被鍨嬭浆鎹€?
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

        if (!card.Keywords.Contains(OshinogoKeywords.Shine))
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

    // 瑙勫垯锛氬厛鍙犲姞闂€€锛屽啀鎸夊浠囦箻绠楋紙鏃犲浠囨寜 1 鍊嶏級銆?
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
