using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using Oshinogo.Scripts.Cards.Ruby;
using Oshinogo.Scripts.Pools.CardPools;
using Oshinogo.Scripts.Pools.PotionPools;
using Oshinogo.Scripts.Pools.RelicPools;
using Oshinogo.Scripts.Relics.Ruby;

public class Aqua : PlaceholderCharacterModel
{

    // 角色名称颜色
    public override Color NameColor => new(0.55f, 0.6f, 0.95f);
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(0.55f, 0.6f, 0.95f);

    // 人物性别（男女中立）
    public override CharacterGender Gender => CharacterGender.Masculine;

    // 初始血量
    public override int StartingHp => 85;

    // 人物模型tscn路径。要自定义见下。
    public override string CustomVisualPath => "res://Oshinogo/scenes/character/aqua.tscn";
    // 卡牌拖尾场景。
    public override string CustomTrailPath => "res://Oshinogo/scenes/vfx/card_trail_aqua.tscn";

    // IconOutlineTexturePath
    public const string IconOutlineTexturePath = "res://Oshinogo/images/ui/aqua_outline.png";

    // 人物头像路径。
    public override string CustomIconTexturePath => "res://Oshinogo/images/ui/aqua.png";

    // 人物头像2号。
    public override string CustomIconPath => "res://Oshinogo/scenes/ui/aqua_icon.tscn";

    // 能量表盘tscn路径。要自定义见下。
    public override string CustomEnergyCounterPath => "res://Oshinogo/scenes/ui/aqua_energy_counter.tscn";


    // 篝火休息场景。
    public override string CustomRestSiteAnimPath => "res://Oshinogo/scenes/rest_site/characters/aqua_rest_site.tscn";


    // 商店人物场景。
    public override string CustomMerchantAnimPath => "res://Oshinogo/scenes/merchant/characters/aqua_merchant.tscn";

    // 多人模式-手指。
    public override string CustomArmPointingTexturePath => "res://Oshinogo/images/ui/hands/multiplayer_hand_aqua_point.png";
    // 多人模式剪刀石头布-石头。
    public override string CustomArmRockTexturePath => "res://Oshinogo/images/ui/hands/multiplayer_hand_aqua_rock.png";
    // 多人模式剪刀石头布-布。
    public override string CustomArmPaperTexturePath => "res://Oshinogo/images/ui/hands/multiplayer_hand_aqua_paper.png";
    // 多人模式剪刀石头布-剪刀。
    public override string CustomArmScissorsTexturePath => "res://Oshinogo/images/ui/hands/multiplayer_hand_aqua_scissors.png";

    // 人物选择背景。
    public override string CustomCharacterSelectBg => "res://Oshinogo/scenes/ui/aqua_background.tscn";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "res://Oshinogo/images/ui/aqua_icon.png";

    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "res://Oshinogo/images/packed/character_select/char_select_aqua_locked.png";


    // 人物选择过渡动画。
    // public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    // 地图上的角色标记图标、表情轮盘上的角色头像
    // public override string CustomMapMarkerPath => null;
    // 攻击音效
    // public override string CustomAttackSfx => null;
    // 施法音效
    // public override string CustomCastSfx => null;
    // 死亡音效
    // public override string CustomDeathSfx => null;
    // 角色选择音效
    // public override string CharacterSelectSfx => null;
    // 过渡音效。这个不能删。
    //public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override CardPoolModel CardPool => ModelDb.CardPool<RubyCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<RubyRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<RubyPotionPool>();

    // 初始卡组
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<IdolAdmiration>(),
        ModelDb.Card<IdolPassion>()

    ];

    // 初始遗物
    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<Photo>(),
    ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}