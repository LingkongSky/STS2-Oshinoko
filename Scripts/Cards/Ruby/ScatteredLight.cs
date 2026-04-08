using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 造成11(13)点伤害，抽3张牌，并免费打出其中的闪耀牌
[Pool(typeof(RubyCardPool))]
public class ScatteredLight : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public ScatteredLight() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);

        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var drawn = (await CardPileCmd.Draw(choiceContext, 3, Owner)).ToList();
        foreach (var card in drawn.Where(c => c.Keywords.Contains(OshinogoKeywords.Shine)))
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
