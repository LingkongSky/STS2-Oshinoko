using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 鍘婚櫎鎵€鏈夐槦鍙嬬殑璐熼潰鏁堟灉銆備笅鍥炲悎浣犱笌鎵€鏈夐槦鍙嬭幏寰?(2)鐐硅兘閲忋€?

[Pool(typeof(RubyCardPool))]
public class BurnedLetter : RubyCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public BurnedLetter() : base(1, CardType.Skill, CardRarity.Event, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var teammates = combatState.GetTeammatesOf(Owner.Creature);
        foreach (var teammate in teammates)
        {
            var debuffs = teammate.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
            foreach (var debuff in debuffs)
            {
                await PowerCmd.Remove(debuff);
            }

            await PowerCmd.Apply<EnergyNextTurnPower>(teammate, DynamicVars.Energy.BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}
