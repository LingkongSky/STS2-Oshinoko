using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Scaffolding.Godot;

namespace Oshinoko.Scripts.Character;

[RegisterCharacter]
public class Ruby : ModCharacterTemplate<RubyCardPool, RubyRelicPool, RubyPotionPool>
{
    public override int StartingGold => 99;
    public override int StartingHp => 75;

    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    // 角色名称颜色
    public override Color NameColor => new(1f, 0.4f, 0.8f);

    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(1f, 0.4f, 0.8f);

    // 地图绘制颜色
    public override Color MapDrawingColor => new(1f, 0.4f, 0.8f);

    // 人物性别
    public override CharacterGender Gender => CharacterGender.Feminine;

    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
        CharacterAssetProfiles.Ironclad(),
        new(
            Scenes: new(
                VisualsPath: "res://Oshinoko/scenes/character/ruby.tscn",
                EnergyCounterPath: "res://Oshinoko/scenes/ui/ruby_energy_counter.tscn",
                MerchantAnimPath: "res://Oshinoko/scenes/merchant/characters/ruby_merchant.tscn",
                RestSiteAnimPath: "res://Oshinoko/scenes/rest_site/characters/ruby_rest_site.tscn"
            ),
            Ui: new(
                IconTexturePath: "res://Oshinoko/images/ui/ruby.png",
                IconOutlineTexturePath: "res://Oshinoko/images/ui/ruby_outline.png",
                IconPath: "res://Oshinoko/scenes/ui/ruby_icon.tscn",
                CharacterSelectBgPath: "res://Oshinoko/scenes/ui/ruby_background.tscn",
                CharacterSelectIconPath: "res://Oshinoko/images/ui/ruby_icon.png",
                CharacterSelectLockedIconPath: "res://Oshinoko/images/packed/character_select/char_select_ruby_locked.png",
                MapMarkerPath: "res://Oshinoko/images/packed/map/icons/map_marker_ruby.png"
            ),
            Vfx: new(
                TrailPath: "res://Oshinoko/scenes/vfx/card_trail_ruby.tscn"
            ),
            Multiplayer: new(
                ArmPointingTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_ruby_point.png",
                ArmRockTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_ruby_rock.png",
                ArmPaperTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_ruby_paper.png",
                ArmScissorsTexturePath: "res://Oshinoko/images/ui/hands/multiplayer_hand_ruby_scissors.png"
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
