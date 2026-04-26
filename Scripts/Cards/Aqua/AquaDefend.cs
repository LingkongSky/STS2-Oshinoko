using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;


[Pool(typeof(AquaCardPool))]
// 描述: 获得5(8)点格挡。
public class AquaDefend : AquaCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),        ];

    public AquaDefend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}

