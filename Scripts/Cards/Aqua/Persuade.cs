using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 每有一名敌人，生成一张浸血花瓣。
public class Persuade : AquaCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Persuade() : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies, true)
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
}

