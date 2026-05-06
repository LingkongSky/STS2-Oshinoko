using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

/// 战斗中每次生成卡牌时，获得临时闪耀。

public class BeautifulDreamPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (Owner.Player == null)
        {
            return;
        }

        if (card.Owner != Owner.Player)
        if (player != Owner.Player || card.Owner != Owner.Player)
        {
            return;
        }

        await ShinePowerHelper.ApplyShine(Owner, Amount, ValueDuration.Temp, Owner, null);
    }
}
