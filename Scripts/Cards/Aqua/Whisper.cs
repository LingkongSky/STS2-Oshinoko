using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Aqua;

[RegisterCard(typeof(AquaCardPool))]
// 描述: 消除一名敌人的人工，并给予2(3)层虚弱
public class Whisper : AquaCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Weak", 2)];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => PlanAndKeywordTips(1, "WEAK");
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Whisper() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        if (cardPlay.Target == null)
        {
            return;
        }

        var artifact = cardPlay.Target.GetPower<ArtifactPower>();
        if (artifact != null)
        {
            await PowerCmd.Remove(artifact);
        }

        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1);
    }
}



