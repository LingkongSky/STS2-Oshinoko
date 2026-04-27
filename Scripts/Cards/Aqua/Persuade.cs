using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 鎻忚堪: 姣忔湁涓€鍚嶆晫浜猴紝鐢熸垚涓€寮犳蹈琛€鑺辩摚銆?
public class Persuade : AquaCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<BloodFlower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Persuade() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CardScope == null)
        {
            return;
        }

        var count = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature).Count() ?? 0;
        for (var i = 0; i < count; i++)
        {
            var bloodFlower = CardScope.CreateCard<BloodFlower>(Owner);
            if (bloodFlower != null)
            {
                await CardPileCmd.Add(bloodFlower, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

}
