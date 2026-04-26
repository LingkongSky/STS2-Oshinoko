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
// 描述: 获得3点格挡3(4)次。
public class ArimaKana : AquaCardModel
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new DynamicVar("Hits", 3)];

    public ArimaKana() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (var i = 0; i < DynamicVars["Hits"].BaseValue; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1);
    }
}

