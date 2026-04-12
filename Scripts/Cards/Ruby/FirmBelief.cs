using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 将临时闪耀转换为回合闪耀，将回合闪耀转换为永久闪耀。

[Pool(typeof(RubyCardPool))]
public class FirmBelief : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FirmBelief() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tempShine = Owner.Creature.GetPowerAmount<TempShinePower>();
        if (tempShine > 0)
        {
            await PowerCmd.ModifyAmount(Owner.Creature.GetPower<TempShinePower>(), -tempShine, Owner.Creature, this);
            await ShinePowerHelper.ApplyShine(Owner.Creature, tempShine, ValueDuration.Turn, Owner.Creature, this);
        }

        var turnShine = Owner.Creature.GetPowerAmount<TurnShinePower>();
        if (turnShine > 0)
        {
            await PowerCmd.ModifyAmount(Owner.Creature.GetPower<TurnShinePower>(), -turnShine, Owner.Creature, this);
            await ShinePowerHelper.ApplyShine(Owner.Creature, turnShine, ValueDuration.Permanent, Owner.Creature, this);
        }
        await CreatureCmd.GainBlock(Owner.Creature, 4, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
