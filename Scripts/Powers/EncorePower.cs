using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinoko.Scripts.Cards.Aqua;

namespace Oshinoko.Scripts.Powers;


/// ÿ�λ�ý�Ѫ����ʱ����ø񵲡�
public class EncorePower : OshinokoCustomPower
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
