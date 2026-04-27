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
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 闅忔満瀵规晫浜洪€犳垚3(4)鐐逛激瀹?娆°€傛湰鍥炲悎鑻ヤ綘鎵撳嚭杩?寮犻棯鑰€鐗岋紝鏀逛负4娆°€?

[Pool(typeof(RubyCardPool))]
public class SpinningStep : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public SpinningStep() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            // No combat state to target opponents.
            return;
        }

        var shinePlaysThisTurn = CombatManager.Instance.History.Entries
            .OfType<CardPlayFinishedEntry>()
            .Count(entry => entry.Actor == Owner.Creature
                && entry.HappenedThisTurn(combatState)
                && entry.CardPlay.Card.Keywords.Contains(OshinogoKeywords.Shine));

        var hitCount = shinePlaysThisTurn >= 2 ? 4 : 3;

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingRandomOpponents(combatState)
            .WithHitCount(hitCount)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}
