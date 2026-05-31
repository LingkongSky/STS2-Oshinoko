using MegaCrit.Sts2.Core.Models.Acts;
using Oshinogo.Scripts.Monsters;

namespace Oshinogo.Scripts.Encounters;

[RegisterActEncounter(typeof(Glory))]
public class KamikiHikaruEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<KamikiHikaru>()];

    public override string BossNodePath => "res://Oshinogo/images/map/placeholder/hikaru";

    public override EncounterAssetProfile AssetProfile => new(
    RunHistoryIconPath: "res://Oshinogo/images/encounters/hikaru.png",
    RunHistoryIconOutlinePath: "res://Oshinogo/images/encounters/hikaru_outline.png");

    public override bool IsWeak => false;

    public override string CustomBgm => "event:/Oshinogo/music/Mephisto";

    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<KamikiHikaru>().ToMutable(), null)
    ];
}
