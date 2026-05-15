using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Oshinogo.Scripts.Encounters
{
    public class AiEncounter : CustomEncounterModel
    {
        public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<HoshinoAi>()];

        public override string CustomRunHistoryIconPath => "res://Oshinogo/images/encounters/ai.png";

        public override string CustomRunHistoryIconOutlinePath => "res://Oshinogo/images/encounters/ai_outline.png";

        // 这个遭遇在那些层级出现
        public override bool IsValidForAct(ActModel act) => act.ActNumber() == 2; // 只在第二幕出现

        // 这个遭遇是否是弱怪池
        public override bool IsWeak => false;

        public AiEncounter() : base(RoomType.Boss)
        {
        }


        // 不要忘了这里的model需要调用ToMutable()，表示不是标准值而是战斗中的可变数据
        protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
            (ModelDb.Monster<HoshinoAi>().ToMutable(), null)
        ];
    }
}
