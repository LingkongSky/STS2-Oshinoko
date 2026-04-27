using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 战斗胜利后，可从卡组中永久移除1张牌。
/// </summary>
public class HandscrollPaintingPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Owner.Player == null)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromDeckForRemoval(
            Owner.Player,
            new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1)
        )).ToList();

        if (selected.Count == 0)
        {
            return;
        }

        await CardPileCmd.RemoveFromDeck(selected);
    }
}
