using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Aqua;

namespace Oshinogo.Scripts.Powers;


/// 每次获得浸血花瓣时，获得格挡。
public class EncorePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (Owner.Player == null)
        {
            return;
        }

        if (card.Owner != Owner.Player || card is not BloodFlower)
        {
            return;
        }

        if (oldPileType == PileType.Hand || card.Pile?.Type != PileType.Hand)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);
    }
}
