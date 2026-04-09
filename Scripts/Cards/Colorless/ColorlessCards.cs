using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Oshinogo.Scripts.Cards.Colorless;

// 描述: 洗去自身的所有Buff，回复10点生命。


[Pool(typeof(ColorlessCardPool))]
public class Rebirth : OshiCardModel
{
    public Rebirth() : base(2, CardType.Skill, CardRarity.Event, TargetType.Self, true)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var buffs = Owner.Creature.Powers.Where(p => p.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Buff).ToList();
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
