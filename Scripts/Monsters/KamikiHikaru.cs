using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Oshinoko.Scripts.Patchs;

namespace Oshinoko.Scripts.Monsters;

[RegisterMonster]
public class KamikiHikaru : ModMonsterTemplate
{
    private enum BossPhase
    {
        Phase1 = 1,
        Phase2 = 2,
    }

    private MoveState? _deadState;
    private MoveState? _phase1InterruptedState;
    private BossPhase _phase = BossPhase.Phase1;
    private int _phase1MoveIndex;
    private int _phase2MoveIndex;
    private int _phase1DamageBase = 15;
    private int _phase1DamageTakenThisCycle;
    private bool _phase1Interrupted;
    private bool _isTransitioning;
    private bool _isReviving;

    public static event Action<KamikiHikaru, int>? PhaseChanged;

    public bool IsInPhase1 => _phase == BossPhase.Phase1;

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 100, 150);

    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 100, 150);


    public override MonsterAssetProfile AssetProfile => new(
        VisualsScenePath: "res://Oshinoko/scenes/monster/hikaru.tscn"
    );

    public override async Task AfterAddedToRoom()
    {
        KamikiHikaruBackgroundPatch.EnsureInitialized();
        await PowerCmd.Apply<KamikiHikaruPhase1StunThresholdPower>(new BlockingPlayerChoiceContext(), Creature, GetPhase1RemainingToInterrupt(), Creature, null, true);
        await PowerCmd.Apply<KamikiHikaruPhase2BuffPurgePower>(new BlockingPlayerChoiceContext(), Creature, 1, Creature, null, true);
        PhaseChanged?.Invoke(this, 1);
    }

    public override bool ShouldStopCombatFromEnding()
    {
        return _phase == BossPhase.Phase1 || _isReviving;
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        if (creature == Creature && _phase == BossPhase.Phase1)
        {
            return false;
        }

        return true;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        if (creature == Creature && _isReviving)
        {
            return false;
        }

        return true;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!wasRemovalPrevented && creature == Creature && _phase == BossPhase.Phase1)
        {
            _isReviving = true;
            return TriggerDeadState();
        }

        return Task.CompletedTask;
    }

    public Task TriggerDeadState()
    {
        if (_deadState == null)
        {
            return Task.CompletedTask;
        }

        SetMoveImmediate(_deadState, forceTransition: true);
        return Task.CompletedTask;
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        _deadState = new MoveState("KAMIKI_PHASE_TRANSITION", RespawnToPhase2Move, new HealIntent(), new BuffIntent())
        {
            MustPerformOnceBeforeTransitioning = true
        };
        _phase1InterruptedState = new MoveState("P1_INTERRUPTED", async _ => await Phase1InterruptedMove(), new StunIntent());

        var p1Move1 = new MoveState("P1_ATTACK_ESCAPE", async _ => await Phase1AttackEscapeMove(), new SingleAttackIntent(() => GetPhase1IntentDamage()), new BuffIntent());
        var p1Move2 = new MoveState("P1_ATTACK_SOULBIND", async _ => await Phase1AttackSoulbindMove(), new SingleAttackIntent(() => GetPhase1IntentDamage()), new DebuffIntent());
        var p1Move3 = new MoveState("P1_ATTACK_FOG", async _ => await Phase1AttackFogMove(), new SingleAttackIntent(() => GetPhase1IntentDamage()), new DebuffIntent());
        var p1Move4 = new MoveState("P1_ATTACK", async _ => await Phase1AttackMove(), new SingleAttackIntent(() => GetPhase1IntentDamage()));

        var p2Move1 = new MoveState("P2_ATTACK_GAIN_REVENGE", async _ => await Phase2AttackGainRevengeMove(), new SingleAttackIntent(() => GetPhase2IntentDamage(10)), new BuffIntent());
        var p2Move2 = new MoveState("P2_ATTACK_WEAK_FRAIL", async _ => await Phase2AttackWeakFrailMove(), new SingleAttackIntent(() => GetPhase2IntentDamage(10)), new DebuffIntent());
        var p2Move3 = new MoveState("P2_ATTACK_TRAP", async _ => await Phase2AttackTrapMove(), new SingleAttackIntent(() => GetPhase2IntentDamage(10)), new DebuffIntent());
        var p2Move4 = new MoveState("P2_ATTACK_VULNERABLE", async _ => await Phase2AttackVulnerableMove(), new SingleAttackIntent(() => GetPhase2IntentDamage(10)), new DebuffIntent());
        var p2Move5 = new MoveState("P2_ATTACK", async _ => await Phase2AttackMove(), new SingleAttackIntent(() => GetPhase2IntentDamage(10)));

        var branch = new ConditionalBranchState("KAMIKI_PHASE_BRANCH");
        branch.AddState(p1Move1, () => _phase == BossPhase.Phase1 && _phase1MoveIndex == 0);
        branch.AddState(p1Move2, () => _phase == BossPhase.Phase1 && _phase1MoveIndex == 1);
        branch.AddState(p1Move3, () => _phase == BossPhase.Phase1 && _phase1MoveIndex == 2);
        branch.AddState(p1Move4, () => _phase == BossPhase.Phase1 && _phase1MoveIndex == 3);

        branch.AddState(p2Move1, () => _phase == BossPhase.Phase2 && _phase2MoveIndex == 0);
        branch.AddState(p2Move2, () => _phase == BossPhase.Phase2 && _phase2MoveIndex == 1);
        branch.AddState(p2Move3, () => _phase == BossPhase.Phase2 && _phase2MoveIndex == 2);
        branch.AddState(p2Move4, () => _phase == BossPhase.Phase2 && _phase2MoveIndex == 3);
        branch.AddState(p2Move5, () => _phase == BossPhase.Phase2 && _phase2MoveIndex == 4);

        _deadState.FollowUpState = branch;
        _phase1InterruptedState.FollowUpState = branch;
        p1Move1.FollowUpState = branch;
        p1Move2.FollowUpState = branch;
        p1Move3.FollowUpState = branch;
        p1Move4.FollowUpState = branch;
        p2Move1.FollowUpState = branch;
        p2Move2.FollowUpState = branch;
        p2Move3.FollowUpState = branch;
        p2Move4.FollowUpState = branch;
        p2Move5.FollowUpState = branch;

        return new MonsterMoveStateMachine([_deadState, _phase1InterruptedState, branch, p1Move1, p1Move2, p1Move3, p1Move4, p2Move1, p2Move2, p2Move3, p2Move4, p2Move5], branch);
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Creature || delta >= 0 || _isTransitioning)
        {
            return Task.CompletedTask;
        }

        if (_phase == BossPhase.Phase1)
        {
            _phase1DamageTakenThisCycle += (int)Math.Ceiling(-delta);
            SyncPhase1StunThresholdPower();

            if (_phase1DamageTakenThisCycle >= GetPhase1InterruptThreshold() && !_phase1Interrupted)
            {
                _phase1Interrupted = true;
                ForcePhase1InterruptedIntent();
            }
        }

        return Task.CompletedTask;
    }

    private void ForcePhase1InterruptedIntent()
    {
        if (_phase != BossPhase.Phase1 || _isTransitioning || _isReviving || _phase1InterruptedState == null)
        {
            return;
        }

        SetMoveImmediate(_phase1InterruptedState, forceTransition: true);
    }

    private async Task RespawnToPhase2Move(IReadOnlyList<Creature> _)
    {
        if (_phase != BossPhase.Phase1)
        {
            return;
        }

        _isTransitioning = true;
        _phase = BossPhase.Phase2;
        _phase2MoveIndex = 0;

        PhaseChanged?.Invoke(this, 2);

        var phase2Hp = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 400, 455);
        await CreatureCmd.SetMaxHp(Creature, phase2Hp);
        await CreatureCmd.Heal(Creature, phase2Hp);
        await RevengePowerHelper.ApplyRevenge(Creature, 1, ValueDuration.Permanent, Creature, null);

        await ClearAllBuffsOnBoard();
        await RemoveBossPower<KamikiHikaruPhase1StunThresholdPower>();
        await RemoveBossPower<KamikiHikaruPhase2BuffPurgePower>();
        _isReviving = false;
        _isTransitioning = false;
    }

    private async Task Phase1AttackEscapeMove()
    {
        if (await TryConsumePhase1Interrupt())
        {
            return;
        }

        await AttackAllPlayers(_phase1DamageBase);
        await PowerCmd.Apply<EscapePower>(new BlockingPlayerChoiceContext(), Creature, 1, Creature, null, true);
        AdvancePhase1Pattern();
    }

    private async Task Phase1AttackSoulbindMove()
    {
        if (await TryConsumePhase1Interrupt())
        {
            return;
        }

        await AttackAllPlayers(_phase1DamageBase);
        await ApplyPowerToAllPlayers<ChainsOfBindingPower>(1);
        AdvancePhase1Pattern();
    }

    private async Task Phase1AttackFogMove()
    {
        if (await TryConsumePhase1Interrupt())
        {
            return;
        }

        await AttackAllPlayers(_phase1DamageBase);
        await ApplyPowerToAllPlayers<SmoggyPower>(1);
        AdvancePhase1Pattern();
    }

    private async Task Phase1AttackMove()
    {
        if (await TryConsumePhase1Interrupt())
        {
            return;
        }

        await AttackAllPlayers(_phase1DamageBase);
        AdvancePhase1Pattern();
    }

    private async Task<bool> TryConsumePhase1Interrupt()
    {
        if (!_phase1Interrupted)
        {
            return false;
        }

        _phase1Interrupted = false;
        AdvancePhase1Pattern();
        SyncPhase1StunThresholdPower();
        return true;
    }

    private Task Phase1InterruptedMove()
    {
        if (_phase1Interrupted)
        {
            _phase1Interrupted = false;
            AdvancePhase1Pattern();
            SyncPhase1StunThresholdPower();
        }

        return Task.CompletedTask;
    }

    private void AdvancePhase1Pattern()
    {
        _phase1MoveIndex = (_phase1MoveIndex + 1) % 4;
        _phase1DamageBase += 5;
        _phase1DamageTakenThisCycle = 0;
        SyncPhase1StunThresholdPower();
    }

    private async Task Phase2AttackGainRevengeMove()
    {
        await Phase2AttackBaseAndSelfDamage(10);
        AdvancePhase2Pattern();
    }

    private async Task Phase2AttackWeakFrailMove()
    {
        await Phase2AttackBaseAndSelfDamage(10);
        await ApplyPowerToAllPlayers<WeakPower>(99);
        await ApplyPowerToAllPlayers<FrailPower>(99);
        AdvancePhase2Pattern();
    }

    private async Task Phase2AttackTrapMove()
    {
        await Phase2AttackBaseAndSelfDamage(10);
        await ApplyPowerToAllPlayers<TrapPower>(99);
        AdvancePhase2Pattern();
    }

    private async Task Phase2AttackVulnerableMove()
    {
        await Phase2AttackBaseAndSelfDamage(10);
        await ApplyPowerToAllPlayers<VulnerablePower>(99);
        AdvancePhase2Pattern();
    }

    private async Task Phase2AttackMove()
    {
        await Phase2AttackBaseAndSelfDamage(10);
        AdvancePhase2Pattern();
    }

    private void AdvancePhase2Pattern()
    {
        _phase2MoveIndex = (_phase2MoveIndex + 1) % 5;
    }

    private async Task Phase2AttackBaseAndSelfDamage(int damage)
    {
        var scaled = GetScaledDamageByRevenge(damage);
        await AttackAllPlayers(scaled);

        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), Creature, scaled, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Creature);
        await RevengePowerHelper.ApplyRevenge(Creature, 1, ValueDuration.Permanent, Creature, null);
    }

    private int GetScaledDamageByRevenge(int baseDamage)
    {
        var revenge = Math.Max(1m, RevengePowerHelper.GetTotalRevenge(Creature));
        return (int)Math.Ceiling((decimal)baseDamage * revenge);
    }

    private decimal GetPhase1IntentDamage()
    {
        return _phase1DamageBase;
    }

    private decimal GetPhase2IntentDamage(int baseDamage)
    {
        var revenge = Math.Max(1m, RevengePowerHelper.GetTotalRevenge(Creature));
        return Math.Ceiling((decimal)baseDamage * revenge);
    }

    private async Task AttackAllPlayers(int damage)
    {
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), playerCreature, damage, ValueProp.Move, Creature);
        }
    }

    private async Task ApplyPowerToAllPlayers<TPower>(int amount) where TPower : PowerModel
    {
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await PowerCmd.Apply<TPower>(new BlockingPlayerChoiceContext(), playerCreature, amount, Creature, null, true);
        }
    }

    private List<Creature> GetPlayerCreatures()
    {
        var combatState = Creature.CombatState;
        return combatState == null
            ? []
            : combatState.Players.Select(p => p.Creature).Where(c => c != null).Select(c => c!).ToList();
    }

    private async Task ClearAllBuffsOnBoard()
    {
        var creatures = GetPlayerCreatures();
        creatures.Add(Creature);

        foreach (var target in creatures)
        {
            foreach (var power in target.Powers.Where(p => p.Type == PowerType.Buff).ToList())
            {
                await PowerCmd.Remove(power);
            }
        }
    }

    private int GetPhase1InterruptThreshold()
    {
        var playerCount = Math.Max(1, Creature.CombatState?.Players.Count ?? 1);
        return 30 * playerCount;
    }

    private int GetPhase1RemainingToInterrupt()
    {
        return Math.Max(0, GetPhase1InterruptThreshold() - _phase1DamageTakenThisCycle);
    }

    private void SyncPhase1StunThresholdPower()
    {
        if (_phase != BossPhase.Phase1 || Creature.CombatState == null)
        {
            return;
        }

        var power = Creature.GetPower<KamikiHikaruPhase1StunThresholdPower>();
        if (power == null)
        {
            return;
        }

        var delta = GetPhase1RemainingToInterrupt() - power.Amount;
        if (delta == 0)
        {
            return;
        }

        TaskHelper.RunSafely(PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, delta, Creature, null, true));
    }

    private async Task RemoveBossPower<TPower>() where TPower : PowerModel
    {
        var power = Creature.GetPower<TPower>();
        if (power != null)
        {
            await PowerCmd.Remove(power);
        }
    }

}
