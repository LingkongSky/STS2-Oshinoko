using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinogo.Scripts.Cards.Colorless;

[Pool(typeof(ColorlessCardPool))]
public class Rebirth : OshiCardModel
{
    public Rebirth() : base(2, CardType.Skill, CardRarity.Event, TargetType.Self, true)
    {
    }

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
