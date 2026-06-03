using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Relics.Ruby;
// 将一张暗号B加入牌库
[RegisterRelic(typeof(RubyRelicPool))]
public class NewBKomachi : OshinokoRelicModel
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



