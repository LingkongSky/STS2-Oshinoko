using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 澶卞幓2鐐圭敓鍛斤紝鑾峰緱20鐐规牸鎸°€傝嫢鏈洖鍚堜綘澶卞幓杩囩敓鍛斤紝鎶?寮犵墝銆?

[Pool(typeof(RubyCardPool))]
public class SwitchToVengeance : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public SwitchToVengeance() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    new BlockVar(20m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        if (CombatHistoryHelper.HasLostHpThisTurn(Owner))
        {
            await CardPileCmd.Draw(choiceContext, 3, Owner);
        }

        await CreatureCmd.Damage(
        choiceContext,
        Owner.Creature,
        2,
        ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
        Owner.Creature
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
