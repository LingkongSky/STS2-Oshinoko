using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Relics.Ruby;
// ��Ǯ��ȡʱ��������20%
[RegisterRelic(typeof(RubyRelicPool))]
public class Mem : OshinokoRelicModel
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



