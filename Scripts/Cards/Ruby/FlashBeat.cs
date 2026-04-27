using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
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

// 鎻忚堪: 閫犳垚7(10)鐐逛激瀹炽€傛湰鍥炲悎鑻ヤ綘浣跨敤杩囬棯鑰€鍊硷紝棰濆閫犳垚7鐐逛激瀹炽€?

[Pool(typeof(RubyCardPool))]
public class FlashBeat : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public FlashBeat() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        var combatState = Owner.Creature.CombatState;
        var hadShine = ShinePowerHelper.GetTotalShine(Owner.Creature) > 0;
        var playedShineEarlier = combatState != null && CombatManager.Instance.History.Entries
            .OfType<CardPlayFinishedEntry>()
            .Any(entry => entry.Actor == Owner.Creature
                && entry.HappenedThisTurn(combatState)
                && entry.CardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine));

        if (hadShine && playedShineEarlier)
        {
            finalDamage += finalDamage;
        }

        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
