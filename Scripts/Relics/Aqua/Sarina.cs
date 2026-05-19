using STS2RitsuLib.Interop.AutoRegistration;

namespace Oshinogo.Scripts.Relics.Aqua;

[RegisterRelic(typeof(AquaRelicPool))]
// 描述: 战斗开始时获得3张浸血花瓣。
public class Sarina : OshinogoRelicModel
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



