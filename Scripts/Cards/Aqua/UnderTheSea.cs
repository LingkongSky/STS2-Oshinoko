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
// 描述: 造成100点伤害，下回合死亡。
public class UnderTheSea : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(100, ValueProp.Move)];

    public UnderTheSea() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<UnderTheSeaPower>(Owner.Creature, 1, Owner.Creature, this);
    }
}

