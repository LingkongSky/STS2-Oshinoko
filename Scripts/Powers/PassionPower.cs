using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Oshinogo.Scripts.Powers;

// 热情：回合结束时增加1层，达到10层时立即死亡��
public class PassionPower : HoshinoAiIconPower
{
    private const int DeathThreshold = 10;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, 1, Owner, null, true);
        if (Amount >= DeathThreshold)
        {
            await CreatureCmd.Kill(Owner, force: true);
        }
    }
}


