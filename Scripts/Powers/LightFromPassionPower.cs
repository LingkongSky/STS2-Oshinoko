using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinoko.Scripts.Powers;

public class LightFromPassionPower : OshinokoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var shine = ShinePowerHelper.GetTotalShine(Owner);
        if (shine <= 0)
        {
            return;
        }

        var block = shine * 6;
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Move, null);
    }
}

