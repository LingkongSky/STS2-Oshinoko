using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 对敌人造成9(12)点伤害，如果造成的伤害大于11(15)则获得2点临时复仇。

[Pool(typeof(RubyCardPool))]
public class Doubt : RubyCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("REVENGE");
    private const string ThresholdKey = "Threshold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar(ThresholdKey, 11),
        new RevengeDynamicVar(2m),
    ];

    public Doubt() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var command = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var dealtDamage = SumDealtDamage(command.Results, cardPlay.Target);
        if (dealtDamage > DynamicVars[ThresholdKey].BaseValue)
        {
            await RevengePowerHelper.ApplyRevenge(
                Owner.Creature,
                DynamicVars[RevengeDynamicVar.Key].BaseValue,
                ValueDuration.Temp,
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars[ThresholdKey].UpgradeValueBy(4);
    }

    private static int SumDealtDamage(IEnumerable<IReadOnlyList<DamageResult>> results, Creature target)
    {
        return results
            .SelectMany(batch => batch)
            .Where(result => result.Receiver == target)
            .Sum(result => result.UnblockedDamage + result.OverkillDamage);
    }
}
