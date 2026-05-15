using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;


public class HoshinoAi : CustomMonsterModel
{
    // 根据进阶提高最小血量，进阶8及以上为120，否则为100
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 45510, 45510);

    // 根据进阶提高最大血量，进阶8及以上为140，否则为120
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 45510, 45510);

    // 意图1的数值，伤害和格挡，根据进阶提高伤害
    private int BasicDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);
    private int BasicBlock => 8;

    // 意图2的数值，重击伤害，根据进阶提高伤害
    private int HeavyDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 24, 20);

    // 怪物场景，如果你的场景没有挂载脚本，参考这个
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene("res://Oshinogo/scenes/monster/ai.tscn");

    public override string CustomVisualPath => "res://Oshinogo/scenes/monster/ai.tscn";


    // 如果你挂载了自己的自定义脚本，使用这个
    // public override string? CustomVisualPath => "res://test/scenes/test_monster.tscn";


    // 战斗开始时，在这里给自己上buff之类
    public override async Task AfterAddedToRoom()
    {

    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        // 意图1：造成伤害，获得格挡
        var basicAttack = new MoveState(
            "BASIC_ATTACK", // 状态ID
            BasicAttackMove, // 执行函数，或者直接用lambda也可
                             // 以下是可变参数，可以填写任意数量的意图，全部展示
            new SingleAttackIntent(BasicDamage),
            new DefendIntent()
        );

        // 意图2：重击
        var heavyAttack = new MoveState(
            "HEAVY_ATTACK",
            async targets => await DamageCmd // 意图2实际执行效果，这里直接用lambda
                .Attack(HeavyDamage)
                .FromMonster(this)
                .WithAttackerFx(null, AttackSfx)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(null),
            new SingleAttackIntent(HeavyDamage)
        );

        // 或者你也可以创建RandomBranchState（随机意图分支）和ConditionalBranchState（条件意图分支）来实现更复杂的状态转换逻辑

        // 设置状态转换，意图1后接意图2，意图2后接意图1
        basicAttack.FollowUpState = heavyAttack;
        heavyAttack.FollowUpState = basicAttack;

        // 添加2个意图，并且初始意图设成 basicAttack
        return new MonsterMoveStateMachine([basicAttack, heavyAttack], basicAttack);
    }

    // 意图1执行实际效果
    private async Task BasicAttackMove(IReadOnlyList<Creature> targets)
    {
        // 说话
        //TalkCmd.Play(L10NMonsterLookup("TEST-TEST_MONSTER.moves.BASIC_ATTACK.banter"), Creature, VfxColor.Blue);

        await DamageCmd
            .Attack(BasicDamage)
            .FromMonster(this)
            // .WithAttackerAnim("Attack", 0.5f) // 如果有攻击动画，可以取消注释并替换成实际动画名称和延迟
            .WithAttackerFx(null, AttackSfx) // 攻击音效
            .WithHitFx("vfx/vfx_attack_blunt") // 攻击特效
            .Execute(null);
        await CreatureCmd.GainBlock(Creature, BasicBlock, ValueProp.Move, null);
    }
}
