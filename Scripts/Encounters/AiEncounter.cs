using MegaCrit.Sts2.Core.Models.Acts;
using Oshinogo.Scripts.Monsters;



namespace Oshinogo.Scripts.Encounters;

[RegisterActEncounter(typeof(Hive))]
public class AiEncounter : ModEncounterTemplate
{
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<HoshinoAi>()];

    public override string BossNodePath => "res://Oshinogo/images/map/placeholder/ai";

    public override EncounterAssetProfile AssetProfile => new(
    RunHistoryIconPath: "res://Oshinogo/images/encounters/ai.png",
    RunHistoryIconOutlinePath: "res://Oshinogo/images/encounters/ai_outline.png");
    public override bool IsWeak => false;

    public override string CustomBgm => "event:/Oshinogo/music/YouAreLight";

    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<HoshinoAi>().ToMutable(), null)
    ];
}


