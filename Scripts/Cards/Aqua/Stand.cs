using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 获得6(9)点格挡，选择一张手牌保留。
public class Stand : AquaCardModel
{
    private const string CalculatedBlockKey = "CalculatedBlock";

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedVar(CalculatedBlockKey, ShineValueType.Block),
    ];

    public Stand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = ShineScaling.Calculate(DynamicVars, CalculatedBlockKey, cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);

        if (PileType.Hand.GetPile(Owner).Cards.Count == 0)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), _ => true, this)).FirstOrDefault();
        selected?.GiveSingleTurnRetain();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}

