using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Scaffolding.Godot;

namespace Oshinoko.Scripts.Character;

[RegisterCharacter]
public class Aqua : ModCharacterTemplate<AquaCardPool, AquaRelicPool, AquaPotionPool>
{
    public override int StartingGold => 99;
    public override int StartingHp => 85;

    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    // 角色名称颜色
    public override Color NameColor => new(0.55f, 0.6f, 0.95f);

    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(0.55f, 0.6f, 0.95f);

    // 地图绘制颜色
    public override Color MapDrawingColor => new(0.55f, 0.6f, 0.95f);

    // 人物性别
    public override CharacterGender Gender => CharacterGender.Masculine;

    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
        CharacterAssetProfiles.Ironclad(),
        new(
            Scenes: new(
                VisualsPath: "res://Oshinoko/scenes/character/aqua.tscn",
                EnergyCounterPath: "res://Oshinoko/scenes/ui/aqua_energy_counter.tscn",
                MerchantAnimPath: "res://Oshinoko/scenes/merchant/characters/aqua_merchant.tscn",
                RestSiteAnimPath: "res://Oshinoko/scenes/rest_site/characters/aqua_rest_site.tscn"
            ),
            Ui: new(
                IconTexturePath: "res://Oshinoko/images/ui/aqua.png",
                IconOutlineTexturePath: "res://Oshinoko/images/ui/aqua_outline.png",
                IconPath: "res://Oshinoko/scenes/ui/aqua_icon.tscn",
                CharacterSelectBgPath: "res://Oshinoko/scenes/ui/aqua_background.tscn",
                CharacterSelectIconPath: "res://Oshinoko/images/ui/aqua_icon.png",
                CharacterSelectLockedIconPath: "res://Oshinoko/images/packed/character_select/char_select_aqua_locked.png",
                MapMarkerPath: "res://Oshinoko/images/packed/map/icons/map_marker_aqua.png"
            ),
            Vfx: new(
                TrailPath: "res://Oshinoko/scenes/vfx/card_trail_aqua.tscn"
            ),
            Multiplayer: new(
                ArmPointingTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_aqua_point.png",
                ArmRockTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_aqua_rock.png",
                ArmPaperTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_aqua_paper.png",
                ArmScissorsTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_aqua_scissors.png"
            )
        )
    );


    // 自动转换人物场景
    protected override NCreatureVisuals? TryCreateCreatureVisuals() =>
        RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(
            AssetProfile.Scenes!.VisualsPath!
        );

    public override bool RequiresEpochAndTimeline => false;

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}
