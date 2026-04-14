using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
        if (Owner?.Creature == null)
        {
            return;
        }

        var turnPower = Owner.Creature.GetPower<TurnShinePower>();
        if (turnPower != null && turnPower.Amount > 0)
        {
            var turnShine = turnPower.Amount;
            await PowerCmd.ModifyAmount(turnPower, -turnShine, Owner.Creature, this);
            await ShinePowerHelper.ApplyShine(Owner.Creature, turnShine, ValueDuration.Permanent, Owner.Creature, this);
        }

        var tempPower = Owner.Creature.GetPower<TempShinePower>();
        if (tempPower != null && tempPower.Amount > 0)
        {
            var tempShine = tempPower.Amount;
            await PowerCmd.ModifyAmount(tempPower, -tempShine, Owner.Creature, this);
            await ShinePowerHelper.ApplyShine(Owner.Creature, tempShine, ValueDuration.Turn, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
