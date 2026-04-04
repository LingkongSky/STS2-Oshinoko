using BaseLib.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oshinogo.Scripts.Pools.PotionPools
{
    public class RubyPotionPool : CustomPotionPoolModel
    {
        // 描述中使用的能量图标。大小为24x24。
        public override string? TextEnergyIconPath => "res://images/ui/ruby_energy.png";
        // tooltip和卡牌左上角的能量图标。大小为74x74。
        public override string? BigEnergyIconPath => "res://images/ruby_energy_big.png";
    }
}
