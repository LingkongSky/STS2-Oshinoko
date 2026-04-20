using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 将你的闪耀值全部转换为复仇值。获得等量能量，并失去2点生命。

[Pool(typeof(RubyCardPool))]
public class DoctorBones : RubyCardModel
{

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    public DoctorBones() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ShinePowerHelper.GetTotalShine(Owner.Creature);
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            2,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature
        );

        if (total > 0)
        {
            await ShinePowerHelper.LoseShine(Owner.Creature, total, Owner.Creature, this);
            await RevengePowerHelper.ApplyRevenge(Owner.Creature, total, ValueDuration.Permanent, Owner.Creature, this);
            await PlayerCmd.GainEnergy(total, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
