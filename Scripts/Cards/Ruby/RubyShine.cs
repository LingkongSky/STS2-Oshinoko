using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 造成3点伤害1次，本场战斗中每使用一次闪耀值，攻击次数+1。

[Pool(typeof(RubyCardPool))]
public class RubyShine : OshiCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(1m),
        new DamageVar(3m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        new CalculatedVar("ShinePlays").WithMultiplier((card, _) =>
            CountShinePlaysForOwner(card.Owner, card)),
    ];

    public RubyShine() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var bonusHits = 0;
        if (ShinePowerHelper.GetTotalShine(Owner.Creature) > 0)
        {
            bonusHits = CountShinePlaysForOwner(Owner, cardPlay.Card);
        }
        var hitCount = 1 + Math.Max(0, bonusHits);

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(hitCount)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    private static int CountShinePlaysForOwner(Player? owner, CardModel? excludeCard)
    {
        if (owner == null)
        {
            return 0;
        }

        var ownerNetId = owner.NetId;
        return CombatManager.Instance.History.Entries
            .OfType<CardPlayFinishedEntry>()
            .Count(entry =>
                entry.Actor?.Player?.NetId == ownerNetId
                && entry.CardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine)
                && (excludeCard == null || entry.CardPlay.Card != excludeCard));
    }
}
