using System.Collections.Generic;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using Oshinogo.Scripts.Cards.Aqua;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics.Aqua;

[Pool(typeof(AquaRelicPool))]
// 描述: 战斗开始时获得3张浸血花瓣。
public class Sarina : OshinogoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeCombatStart()
    {
        if (Owner?.Creature?.CombatState == null)
        {
            return;
        }

        Flash();
        var generated = new List<CardPileAddResult>();
        for (var i = 0; i < 3; i++)
        {
            var bloodFlower = Owner.Creature.CombatState.CreateCard<BloodFlower>(Owner);
            generated.Add(await CardPileCmd.AddGeneratedCardToCombat(bloodFlower, PileType.Hand, Owner, CardPilePosition.Top));
        }

        CardCmd.PreviewCardPileAdd(generated);
    }
}
