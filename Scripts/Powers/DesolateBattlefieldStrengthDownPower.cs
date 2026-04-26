using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Oshinogo.Scripts.Powers;

/// <summary>
/// 本回合临时失去力量，回合结束时返还。
/// </summary>
public class DesolateBattlefieldStrengthDownPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterApplied(MegaCrit.Sts2.Core.Entities.Creatures.Creature? applier, MegaCrit.Sts2.Core.Models.CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(Owner, -Amount, applier, cardSource, silent: true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null, silent: true);
        await PowerCmd.Remove(this);
    }
}

