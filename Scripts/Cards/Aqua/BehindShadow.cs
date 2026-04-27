using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Powers;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 失去2点生命，获得2点回合复仇，翻倍一名敌人的负面效果层数。
public class BehindShadow : AquaCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => KeywordTips("REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public BehindShadow() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            2,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            Owner.Creature
        );

        await RevengePowerHelper.ApplyRevenge(Owner.Creature, 2, ValueDuration.Turn, Owner.Creature, this);

        if (cardPlay.Target == null)
        {
            return;
        }

        var debuffs = cardPlay.Target.Powers.Where(power => power.Type == PowerType.Debuff && power.Amount > 0).ToList();
        foreach (var debuff in debuffs)
        {
            await PowerCmd.ModifyAmount(debuff, debuff.Amount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

