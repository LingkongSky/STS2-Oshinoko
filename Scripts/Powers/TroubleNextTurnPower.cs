using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using Oshinogo.Scripts.Cards.Aqua;

namespace Oshinogo.Scripts.Powers;


// 下回合开始时获得浸血花瓣�?
public class TroubleNextTurnPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null || Owner.Player == null)
        {
            await PowerCmd.Remove(this);
            return;
        }

        for (var i = 0; i < Amount; i++)
        {
            var card = combatState.CreateCard<BloodFlower>(Owner.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player, CardPilePosition.Top);
        }

        await PowerCmd.Remove(this);
    }
}
