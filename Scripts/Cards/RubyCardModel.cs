public abstract class RubyCardModel : ModCardTemplate
{
    public override string PortraitPath => $"res://Oshinoko/images/cards/ruby/{GetType().Name}.png";


    public RubyCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected static IEnumerable<IHoverTip> KeywordTips(params string[] keys)
    {
        return CardKeywordHoverTipHelper.Create(keys);
    }

    public override CardAssetProfile AssetProfile => new(
    FramePath: Type switch
    {
        CardType.Attack =>
            "res://Oshinoko/images/ui/card_frame/ruby_attack.png",

        CardType.Power =>
            "res://Oshinoko/images/ui/card_frame/ruby_power.png",

        _ =>
            "res://Oshinoko/images/ui/card_frame/ruby_skill.png"
    }
);


    protected static IEnumerable<IHoverTip> MergeKeywordTips(
        IEnumerable<IHoverTip> primary,
        params string[] keys
    )
    {
        return CardKeywordHoverTipHelper.Merge(
            primary,
            CardKeywordHoverTipHelper.Create(keys)
        );
    }

    protected static IEnumerable<IHoverTip> PlanAndKeywordTips(
        int amount,
        params string[] keys
    )
    {
        return CardKeywordHoverTipHelper.Merge(
            PlanCostHelper.CreatePlanCostHoverTips(amount),
            CardKeywordHoverTipHelper.Create(keys)
        );
    }


}


