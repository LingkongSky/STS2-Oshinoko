using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得3点回合复仇。若本回合你每次失去生命，令所有敌人失去5点生命。

[RegisterCard(typeof(RubyCardPool))]
public class MothersLie : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public MothersLie() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await RevengePowerHelper.ApplyRevenge(Owner.Creature, 3, ValueDuration.Turn, Owner.Creature, this);
        await PowerCmd.Apply<MothersLiePower>(choiceContext, Owner.Creature, 5, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}



