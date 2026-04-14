using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Cards.Ruby;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics.Ruby;
// 将一张暗号B加入牌库
[Pool(typeof(RubyRelicPool))]
public class NewBKomachi : RubyRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool HasUponPickupEffect => true;


    public override async Task AfterObtained()
    {
        if (Owner == null)
        {
            return;
        }

        var card = Owner.RunState.CreateCard(ModelDb.Card<CodeB>(), Owner);
        List<CardPileAddResult> results = new List<CardPileAddResult>
        {
            await CardPileCmd.Add(card, PileType.Deck)
        };

        CardCmd.PreviewCardPileAdd(results);
    }
}
