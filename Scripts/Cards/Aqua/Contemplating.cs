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
// 描述: 获得8(11)点格挡，给予2(3)层流言。
public class Contemplating : AquaCardModel
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move), new DynamicVar("Rumor", 2)];

    public Contemplating() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<RumorPower>(cardPlay.Target, DynamicVars["Rumor"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars["Rumor"].UpgradeValueBy(1);
    }
}

