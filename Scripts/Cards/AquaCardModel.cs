public abstract class AquaCardModel : ModCardTemplate
{
    public override string PortraitPath => $"res://Oshinoko/images/cards/aqua/{GetType().Name}.png";


    public AquaCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary) : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override CardAssetProfile AssetProfile => new(
    FramePath: Type switch
    {
        CardType.Attack =>
            "res://Oshinoko/images/ui/card_frame/aqua_attack.png",

        CardType.Power =>
            "res://Oshinoko/images/ui/card_frame/aqua_power.png",

        _ =>
            "res://Oshinoko/images/ui/card_frame/aqua_skill.png"
    }
);


    protected static IEnumerable<IHoverTip> KeywordTips(params string[] keys)
    {
        return CardKeywordHoverTipHelper.Create(keys);
    }

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


