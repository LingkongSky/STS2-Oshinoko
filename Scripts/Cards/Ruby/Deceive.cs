using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得8(11)点格挡。若本回合你已失去过生命，获得1点临时复仇值。

[Pool(typeof(RubyCardPool))]
public class Deceive : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("REVENGE");
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new RevengeDynamicVar(1m),
    ];

    public Deceive() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (CombatHistoryHelper.HasLostHpThisTurn(Owner))
        {
            await RevengePowerHelper.ApplyRevenge(Owner.Creature, DynamicVars[RevengeDynamicVar.Key].BaseValue, ValueDuration.Temp, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
