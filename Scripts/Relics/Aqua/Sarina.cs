using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinoko.Scripts.Relics.Aqua;

[RegisterRelic(typeof(AquaRelicPool))]
// ����: ս����ʼʱ���3�Ž�Ѫ���ꡣ
public class Sarina : OshinokoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeCombatStart()
    {
        if (Owner?.Creature?.CombatState == null)
        {
            return;
        }

        Flash();
        var generated = new List<CardPileAddResult>();
        for (var i = 0; i < 3; i++)
        {
            var bloodFlower = Owner.Creature.CombatState.CreateCard<BloodFlower>(Owner);
            generated.Add(await CardPileCmd.AddGeneratedCardToCombat(bloodFlower, PileType.Hand, Owner, CardPilePosition.Top));
        }

        CardCmd.PreviewCardPileAdd(generated);
    }
}



