using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 缁欎簣鎵€鏈夋晫浜?(2)灞傝櫄寮卞拰1(2)灞傛槗浼ゃ€?

[Pool(typeof(RubyCardPool))]
public class Disdain : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("VULNERABLE", "WEAK");
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Weak", 1),
        new DynamicVar("Vulnerable", 1),
    ];

    public Disdain() : base(1, CardType.Skill, CardRarity.Event, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var opponents = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature) ?? Enumerable.Empty<Creature>();
        foreach (var enemy in opponents)
        {
            await PowerCmd.Apply<WeakPower>(enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1);
        DynamicVars["Vulnerable"].UpgradeValueBy(1);
    }
}
