using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Cards.Ruby;

// 描述: 回合结束时对所有敌人造成复仇*6的伤害。若本回合你失去过生命，伤害+8。

[RegisterCard(typeof(RubyCardPool))]
public class ShellForgedByLies : RubyCardModel
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => KeywordTips("REVENGE");
    public ShellForgedByLies() : base(2, CardType.Power, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ShellForgedByLiesPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}


