using MegaCrit.Sts2.Core.Models.CardPools;

namespace Oshinoko.Scripts.Cards.Colorless;

// 洗去自身的所有Buff，回复10点生命
[RegisterCard(typeof(ColorlessCardPool))]
public class Rebirth : ModCardTemplate
{
    public override string PortraitPath => $"res://Oshinoko/images/cards/ruby/{GetType().Name}.png";

    public Rebirth() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
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







