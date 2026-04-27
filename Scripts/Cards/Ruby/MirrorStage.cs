using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鑾峰緱14(18)鐐规牸鎸★紝鏈洖鍚堝弽寮规墍鏈夎鏍兼尅鐨勪激瀹炽€?

[Pool(typeof(RubyCardPool))]
public class MirrorStage : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(14m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar("CalculatedBlock", ShineValueType.Block),
    ];

    public MirrorStage() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Use calculated block so shine bonuses apply.
        var blockValue = ShineScaling.Calculate(DynamicVars, "CalculatedBlock", Owner.Creature);
        await CreatureCmd.GainBlock(Owner.Creature, blockValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<MirrorStagePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}
