using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Colorless;

// 鎻忚堪: 娲楀幓鑷韩鐨勬墍鏈塀uff锛屽洖澶?0鐐圭敓鍛姐€?
[Pool(typeof(ColorlessCardPool))]
public class Rebirth : CustomCardModel
{
    public override string PortraitPath => $"res://Oshinogo/images/cards/ruby/{GetType().Name}.png";

    public Rebirth() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var buffs = Owner.Creature.Powers.ToList();
        foreach (var buff in buffs)
        {
            await PowerCmd.Remove(buff);
        }

        await CreatureCmd.Heal(Owner.Creature, 10, playAnim: true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
