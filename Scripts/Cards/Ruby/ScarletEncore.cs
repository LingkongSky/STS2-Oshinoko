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

// 鎻忚堪: 瀵规墍鏈夋晫浜洪€犳垚8(10)鐐逛激瀹炽€傝嫢鏈洖鍚堟娊杩囩墝锛屽垯鏀逛负閫犳垚2娆°€?

[Pool(typeof(RubyCardPool))]
public class ScarletEncore : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
    ];

    public ScarletEncore() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, true)
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

        var drewThisTurn = CombatManager.Instance.History.Entries
            .OfType<CardDrawnEntry>()
            .Any(entry => entry.Actor == Owner.Creature
                && entry.RoundNumber == combatState.RoundNumber
                && entry.CurrentSide == combatState.CurrentSide
                && !entry.FromHandDraw);

        var hitCount = drewThisTurn ? 2 : 1;

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .TargetingAllOpponents(combatState)
            .WithHitCount(hitCount)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
