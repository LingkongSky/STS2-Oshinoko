using MegaCrit.Sts2.Core.Models.Acts;
using Oshinoko.Scripts.Monsters;

namespace Oshinoko.Scripts.Encounters;

[RegisterActEncounter(typeof(Glory))]
public class KamikiHikaruEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<KamikiHikaru>()];

    public override string BossNodePath => "res://Oshinoko/images/map/placeholder/hikaru";

    public override EncounterAssetProfile AssetProfile => new(
    RunHistoryIconPath: "res://Oshinoko/images/encounters/hikaru.png",
    RunHistoryIconOutlinePath: "res://Oshinoko/images/encounters/hikaru_outline.png");

    public override bool IsWeak => false;

    public override string CustomBgm => "event:/Oshinoko/music/Mephisto";

    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<KamikiHikaru>().ToMutable(), null)
    ];
}
