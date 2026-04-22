using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Oshinogo.Scripts.Powers;

public class LightFromPassionPower : OshinogoCustomPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
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
