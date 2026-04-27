using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鎵€鏈夐槦鍙嬭幏寰?鐐硅兘閲忓苟鎶?寮犵墝銆?

[Pool(typeof(RubyCardPool))]
public class SpotlightShare : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    public SpotlightShare() : base(2, CardType.Skill, CardRarity.Event, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var teammates = combatState.GetTeammatesOf(Owner.Creature);
        foreach (var teammate in teammates)
        {
            teammate.Player?.PlayerCombatState?.GainEnergy(1);
            if (teammate.Player != null)
            {
                await CardPileCmd.Draw(choiceContext, 1, teammate.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
