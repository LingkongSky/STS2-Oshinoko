using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 闅忔満娑堣€楁墜鐗屼腑鐨?寮犲崱锛屽苟鑾峰緱14(18)鐐规牸鎸°€?

[Pool(typeof(RubyCardPool))]
public class NoWayBack : RubyCardModel
{
    public NoWayBack() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
[
    new BlockVar(14m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards;
        if (hand.Count == 0)
        {
            return;
        }

        var rng = Owner.RunState.Rng.CombatCardSelection;
        var selected = rng.NextItem(hand);
        if (selected == null)
        {
            return;
        }


        await CardCmd.Exhaust(choiceContext, selected);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);


    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Block.UpgradeValueBy(4);

    }
}
