using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 每抽到指定数量的牌后，额外抽1张牌。
/// </summary>
public class UnreachablePower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _drawCounter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (Owner.Player == null || card.Owner != Owner.Player)
        {
            return;
        }

        _drawCounter++;
        var threshold = Math.Max(1, Amount);
        if (_drawCounter < threshold)
        {
            return;
        }

        _drawCounter -= threshold;
        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}
