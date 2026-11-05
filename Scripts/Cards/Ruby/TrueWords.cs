using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Cards.Ruby;

// ����: ȥ��һ�����˵����з����������������15������1����ҫ����1���ơ�

[RegisterCard(typeof(RubyCardPool))]
public class TrueWords : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("SHINE");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public TrueWords() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var block = cardPlay.Target.Block;
        if (block > 0)
        {
            await CreatureCmd.LoseBlock(choiceContext, cardPlay.Target, block, Owner.Creature);
        }

        if (block > 15)
        {
            await ShinePowerHelper.ApplyShine(Owner.Creature, 1, ValueDuration.Permanent, Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}



