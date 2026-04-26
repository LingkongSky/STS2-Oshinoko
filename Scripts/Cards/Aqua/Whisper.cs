using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Cards.Other;
using Oshinogo.Scripts.Pools.CardPools;

namespace Oshinogo.Scripts.Cards.Aqua;

[Pool(typeof(AquaCardPool))]
// 描述: 消除一名敌人的人工，并给予2(3)层虚弱 谋划1
public class Whisper : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Weak", 2)];
    protected override IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> ExtraHoverTips => PlanCostHelper.CreatePlanCostHoverTips(1);

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Whisper() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override bool IsPlayable => PlanCostHelper.HasEnoughPlan(Owner, 1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await PlanCostHelper.TryConsumePlan(Owner, this, 1))
        {
            return;
        }

        if (cardPlay.Target == null)
        {
            return;
        }

        var artifact = cardPlay.Target.GetPower<ArtifactPower>();
        if (artifact != null)
        {
            await PowerCmd.Remove(artifact);
        }

        await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1);
    }
}
