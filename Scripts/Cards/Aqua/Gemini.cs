using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 获得2点回合闪耀和2点回合复仇。
public class Gemini : AquaCardModel
{
    public Gemini() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ShinePowerHelper.ApplyShine(Owner.Creature, 2, ValueDuration.Turn, Owner.Creature, this);
        await RevengePowerHelper.ApplyRevenge(Owner.Creature, 2, ValueDuration.Turn, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

