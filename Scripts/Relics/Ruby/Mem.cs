using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using Oshinogo.Scripts.Pools.RelicPools;

namespace Oshinogo.Scripts.Relics.Ruby;
// 金钱获取时额外增加20%
[Pool(typeof(RubyRelicPool))]
public class Mem : RubyRelicModel
{
    private const decimal BonusMultiplier = 0.2m;

    private decimal _pendingBonusGold;

    private bool _isApplyingBonus;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool ShouldGainGold(decimal amount, Player player)
    {
        if (_isApplyingBonus)
        {
            return true;
        }

        if (player != Owner)
        {
            return true;
        }

        _pendingBonusGold = Math.Floor(amount * BonusMultiplier);
        return true;
    }

    public override async Task AfterGoldGained(Player player)
    {
        if (player != Owner || _isApplyingBonus || _pendingBonusGold <= 0m)
        {
            return;
        }

        var pendingBonusGold = _pendingBonusGold;
        _pendingBonusGold = 0m;
        _isApplyingBonus = true;
        Flash();
        await PlayerCmd.GainGold(pendingBonusGold, Owner);
        _isApplyingBonus = false;
    }
}
