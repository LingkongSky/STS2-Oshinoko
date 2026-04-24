using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace Oshinogo.Scripts.Pools.CardPools
{
    public class AquaCardPool : CustomCardPoolModel
    {
        public override string Title => "Aqua";

        public override string? TextEnergyIconPath => "res://Oshinogo/images/powers/aqua_energy.png";
        public override string? BigEnergyIconPath => "res://Oshinogo/images/powers/aqua_energy_big.png";

        public override Color DeckEntryCardColor => new(0f, 0f, 0f, 1f);

        public override Texture2D? CustomFrame(CustomCardModel card)
        {
            return card.Type switch
            {
                CardType.Attack => PreloadManager.Cache.GetAsset<Texture2D>("res://Oshinogo/images/ui/card_frame/aqua_attack.png"),
                CardType.Power => PreloadManager.Cache.GetAsset<Texture2D>("res://Oshinogo/images/ui/card_frame/aqua_power.png"),
                _ => PreloadManager.Cache.GetAsset<Texture2D>("res://Oshinogo/images/ui/card_frame/aqua_skill.png"),
            };
        }

        public override bool IsColorless => false;

    }
}
