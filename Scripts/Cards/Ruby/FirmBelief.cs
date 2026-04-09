using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 将当前闪耀值减半后转换为回合闪耀值，获得4点格挡。

[Pool(typeof(RubyCardPool))]
public class FirmBelief : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FirmBelief() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var total = ShinePowerHelper.GetTotalShine(Owner.Creature);
        var converted = total / 2;
        if (total > 0)
        {
            await ShinePowerHelper.LoseShine(Owner.Creature, total, Owner.Creature, this);
        }
        if (converted > 0)
        {
            await ShinePowerHelper.ApplyShine(Owner.Creature, converted, ValueDuration.Turn, Owner.Creature, this);
        }
        await CreatureCmd.GainBlock(Owner.Creature, 4, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
