using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 获得1(2)层人工制品，战斗结束后随机升级一张卡牌

[RegisterCard(typeof(RubyCardPool))]
public class ChannelSubscribe : RubyCardModel
{
    private const string ArtifactKey = "Artifact";
    private bool _playedThisCombat;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];



    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ArtifactKey, 1),
    ];


    public ChannelSubscribe() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _playedThisCombat = true;
        await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature, DynamicVars[ArtifactKey].BaseValue, Owner.Creature, this, true);
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        if (!_playedThisCombat)
        {
            return Task.CompletedTask;
        }

        var upgradable = PileType.Deck.GetPile(Owner).Cards
            .Where(card => card?.IsUpgradable ?? false)
            .ToList();

        if (upgradable.Count > 0)
        {
            var rng = Owner.RunState.Rng.CombatCardSelection;
            var selected = rng.NextItem(upgradable);
            if (selected != null)
            {
                CardCmd.Upgrade(selected);
            }
        }

        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        _playedThisCombat = false;
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ArtifactKey].UpgradeValueBy(1);
        RemoveKeyword(CardKeyword.Ethereal);
    }
}




