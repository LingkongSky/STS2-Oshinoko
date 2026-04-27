using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 閫犳垚8(10)鐐逛激瀹炽€傝嫢閫犳垚鐨勪激瀹冲ぇ浜?2(15)锛岃繑杩?(2)鐐硅垂鐢ㄣ€?

[Pool(typeof(RubyCardPool))]
public class HeartFlutterAttack : RubyCardModel
{
    private const string ThresholdKey = "Threshold";
    private const string RefundEnergyKey = "RefundEnergy";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [OshinogoKeywords.Shine];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CalculationExtraVar(1m),
        ShineScaling.CreateCalculatedDamageVar(ValueProp.Move),
        new DynamicVar(ThresholdKey, 12),
        new DynamicVar(RefundEnergyKey, 1),
    ];

    public HeartFlutterAttack() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var finalDamage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);

        var command = await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        var dealtDamage = SumDealtDamage(command.Results, cardPlay.Target);
        if (dealtDamage > DynamicVars[ThresholdKey].BaseValue)
        {
            await PlayerCmd.GainEnergy(DynamicVars[RefundEnergyKey].BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars[ThresholdKey].UpgradeValueBy(3);
        DynamicVars[RefundEnergyKey].UpgradeValueBy(1);
    }

    private static int SumDealtDamage(IEnumerable<DamageResult> results, Creature target)
    {
        return results
            .Where(r => r.Receiver == target)
            .Sum(r => r.UnblockedDamage + r.OverkillDamage);
    }
}
