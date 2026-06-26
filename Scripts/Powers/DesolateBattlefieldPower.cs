using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Oshinoko.Scripts.Powers;

/// <summary>
/// 本回合临时失去力量，回合结束时返还
/// </summary>
public class DesolateBattlefieldPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(MegaCrit.Sts2.Core.Entities.Creatures.Creature? applier, MegaCrit.Sts2.Core.Models.CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), Owner, -Amount, applier, cardSource, false);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null, false);
        await PowerCmd.Remove(this);
    }
}


