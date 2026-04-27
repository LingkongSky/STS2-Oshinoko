using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

[Pool(typeof(RubyCardPool))]
// 浠庢秷鑰楃墝鍫嗕腑閫夋嫨1(2)寮犵墝鍔犲叆鎵嬬墝锛岃嚦灏戦渶瑕?鐐归棯鑰€鎵嶈兘鎵撳嚭
public class FindTruth : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    protected override bool IsPlayable => Owner?.Creature != null
        && ShinePowerHelper.GetTotalShine(Owner.Creature) > 3;

    public FindTruth() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        if (exhaustPile.Cards.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, exhaustPile.Cards.ToList(), Owner, prefs);
        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
