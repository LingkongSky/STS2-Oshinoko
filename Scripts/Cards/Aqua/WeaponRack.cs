using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 造成4(6)点伤害2次，每造成未被格挡的伤害时获得一张浸血花瓣。
public class WeaponRack : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];

    public WeaponRack() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var command = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(2)
            .Execute(choiceContext);

        var procCount = CountUnblockedHits(command.Results, cardPlay.Target);
        if (CardScope == null || procCount <= 0)
        {
            return;
        }

        for (var i = 0; i < procCount; i++)
        {
            var bloodFlower = CardScope.CreateCard<BloodFlower>(Owner);
            if (bloodFlower != null)
            {
                await CardPileCmd.Add(bloodFlower, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }

    private static int CountUnblockedHits(IEnumerable<DamageResult> results, Creature target)
    {
        return results.Count(result => result.Receiver == target && result.UnblockedDamage > 0);
    }
}

