using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 将闪耀值全部转换为复仇值
[Pool(typeof(RubyCardPool))]
public class DoctorBones : OshiCardModel
{
    public DoctorBones() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ShinePowerHelper.GetTotalShine(Owner.Creature);
        if (total <= 0)
        {
            return;
        }

        await ShinePowerHelper.LoseShine(Owner.Creature, total, Owner.Creature, this);
        await RevengePowerHelper.ApplyRevenge(Owner.Creature, total, ValueDuration.Permanent, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
