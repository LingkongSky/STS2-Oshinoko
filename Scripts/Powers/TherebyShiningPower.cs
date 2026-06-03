using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Oshinoko.Scripts.Powers;


/// ÿ�δ��������ʱ�����1��������
public class TherebyShiningPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
        {
            return;
        }

        if (!cardPlay.Card.Keywords.Contains(CardKeyword.Ethereal))
        {
            return;
        }

        if (Owner.Player == null)
        {
            return;
        }

        await PlayerCmd.GainEnergy(1, Owner.Player);
    }
}
