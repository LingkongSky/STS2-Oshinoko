using MegaCrit.Sts2.Core.Entities.Merchant;

namespace Oshinoko.Scripts.Relics.Ruby;

// 每次获得金钱时，额外获得15金钱。
[RegisterRelic(typeof(RubyRelicPool))]
public class Mem : OshinokoRelicModel
{
    private const int BonusGold = 15;

    private int _lastObservedGold;
    private bool _isApplyingBonus;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override Task AfterObtained()
    {
        _lastObservedGold = Owner?.Gold ?? 0;
        return Task.CompletedTask;
    }

    public override async Task AfterGoldGained(Player player)
    {
        if (player != Owner)
        {
            return;
        }

        if (_isApplyingBonus)
        {
            _lastObservedGold = player.Gold;
            return;
        }

        var delta = player.Gold - _lastObservedGold;
        _lastObservedGold = player.Gold;
        if (delta <= 0)
        {
            return;
        }

        _isApplyingBonus = true;
        Flash();
        await PlayerCmd.GainGold(BonusGold, Owner);
        _lastObservedGold = Owner.Gold;
        _isApplyingBonus = false;
    }

    public override Task AfterItemPurchased(Player player, MerchantEntry itemPurchased, int goldSpent)
    {
        if (player == Owner)
        {
            _lastObservedGold = player.Gold;
        }

        return Task.CompletedTask;
    }
}
