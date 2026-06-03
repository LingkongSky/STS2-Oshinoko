using MegaCrit.Sts2.Core.Models.Acts;
using Oshinoko.Scripts.Monsters;

namespace Oshinoko.Scripts.Encounters;

[RegisterActEncounter(typeof(Hive))]
public class AiEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<HoshinoAi>()];

    public override string BossNodePath => "res://Oshinoko/images/map/placeholder/ai";

    public override EncounterAssetProfile AssetProfile => new(
    RunHistoryIconPath: "res://Oshinoko/images/encounters/ai.png",
    RunHistoryIconOutlinePath: "res://Oshinoko/images/encounters/ai_outline.png");
    public override bool IsWeak => false;

    public override string CustomBgm => "event:/Oshinoko/music/YouAreLight";

    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<HoshinoAi>().ToMutable(), null)
    ];
}


