using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 造成7(10)点伤害，获得2点临时复仇。
public class BloodSword : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new RevengeDynamicVar(2),
    ];

    public BloodSword() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await RevengePowerHelper.ApplyRevenge(Owner.Creature, DynamicVars[RevengeDynamicVar.Key].BaseValue, ValueDuration.Temp, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
