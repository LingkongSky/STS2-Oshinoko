using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 下回合开始时对敌人造成10(14)点伤害
public class Faraway : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];

    public Faraway() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<FarawayPower>(cardPlay.Target, DynamicVars.Damage.BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}

