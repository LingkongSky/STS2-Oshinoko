using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Oshinogo.Scripts.Pools.CardPools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Oshinogo.Scripts.Cards.Ruby;

// 鎻忚堪: 锛岃幏寰?(2)灞備汉宸ュ埗鍝侊紝鎴樻枟缁撴潫鍚庨殢鏈哄崌绾т竴寮犲崱鐗屻€?

[Pool(typeof(RubyCardPool))]
public class ChannelSubscribe : RubyCardModel
{
    private const string ArtifactKey = "ArtifactPower";
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
        await PowerCmd.Apply<ArtifactPower>(Owner.Creature, DynamicVars[ArtifactKey].BaseValue, Owner.Creature, this);
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
