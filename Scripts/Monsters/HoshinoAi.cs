using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Oshinogo.Scripts.Cards.Colorless;
using Oshinogo.Scripts.Patchs;
using Oshinogo.Scripts.Relics.Aqua;
using Oshinogo.Scripts.Relics.Ruby;

namespace Oshinogo.Scripts.Monsters;

[RegisterMonster]
public class HoshinoAi : ModMonsterTemplate
{
    private enum BossPhase
    {
        Phase1 = 1,
        Phase2 = 2,
        Phase3 = 3,
        Complete = 4,
    }

    private BossPhase _phase = BossPhase.Phase1;
    private bool _phase1UseEnergy = true;
    private bool _phase2UseOrbit = true;
    private bool _phase3UseRebirthCard = true;
    private bool _killedByPlayerHookTriggered;
    private bool _completedAllTasksRewardIssued;
    private readonly HashSet<Creature> _phase3RebirthPlayedPlayers = new();

    public static event Action<HoshinoAi, int>? PhaseChanged;

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 200, 200);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 255, 255);

    public override MonsterAssetProfile AssetProfile => new(
        VisualsScenePath: "res://Oshinogo/scenes/monster/ai.tscn"
    );

    public override async Task AfterAddedToRoom()
    {
        HoshinoAiBackgroundPatch.EnsureInitialized();

        var playerCount = GetPlayerCreatures().Count;
        await ApplyPowerToAllPlayers<PassionPower>(1);

        await PowerCmd.Apply<HoshinoAiMechanicsPower>(new BlockingPlayerChoiceContext(), Creature, 1, Creature, null, true);
        await EnterPhase1(playerCount);
    }



    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var phase1Energy = new MoveState("PHASE1_ENERGY", async _ => await Phase1GiveEnergyMove(), new BuffIntent());
        var phase1Draw = new MoveState("PHASE1_DRAW", async _ => await Phase1GiveDrawMove(), new BuffIntent());
        var phase2Orbit = new MoveState("PHASE2_ORBIT", async _ => await Phase2GiveOrbitMove(), new BuffIntent());
        var phase2Stats = new MoveState("PHASE2_STATS", async _ => await Phase2GiveStatsMove(), new BuffIntent());
        var phase3Rebirth = new MoveState("PHASE3_REBIRTH_CARD", async _ => await Phase3GiveRebirthMove(), new StatusIntent(GetPlayerCountForIntent()));
        var phase3NoDraw = new MoveState("PHASE3_NODRAW", async _ => await Phase3NoDrawMove(), new DebuffIntent());

        var branch = new ConditionalBranchState("PHASE_BRANCH");
        branch.AddState(phase1Energy, () => _phase == BossPhase.Phase1 && _phase1UseEnergy);
        branch.AddState(phase1Draw, () => _phase == BossPhase.Phase1 && !_phase1UseEnergy);
        branch.AddState(phase2Orbit, () => _phase == BossPhase.Phase2 && _phase2UseOrbit);
        branch.AddState(phase2Stats, () => _phase == BossPhase.Phase2 && !_phase2UseOrbit);
        branch.AddState(phase3Rebirth, () => _phase == BossPhase.Phase3 && _phase3UseRebirthCard);
        branch.AddState(phase3NoDraw, () => _phase == BossPhase.Phase3 && !_phase3UseRebirthCard);

        phase1Energy.FollowUpState = branch;
        phase1Draw.FollowUpState = branch;
        phase2Orbit.FollowUpState = branch;
        phase2Stats.FollowUpState = branch;
        phase3Rebirth.FollowUpState = branch;
        phase3NoDraw.FollowUpState = branch;

        return new MonsterMoveStateMachine([branch, phase1Energy, phase1Draw, phase2Orbit, phase2Stats, phase3Rebirth, phase3NoDraw], branch);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        var ownerCreature = card.Owner?.Creature;
        if (_phase != BossPhase.Phase1 || ownerCreature?.Player == null)
        {
            return;
        }

        await DecreaseGoalPower<HoshinoAiPhase1DrawGoalPower>(1, ownerCreature);
        await TryAdvancePhase();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var ownerCreature = cardPlay.Card.Owner?.Creature;
        if (ownerCreature?.Player == null)
        {
            return;
        }

        switch (_phase)
        {
            case BossPhase.Phase1:
                await DecreaseGoalPower<HoshinoAiPhase1PlayGoalPower>(1, ownerCreature);
                await TryAdvancePhase();
                return;
            case BossPhase.Phase2:
                var spent = (int)Math.Max(0, cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.All));
                if (spent > 0)
                {
                    await DecreaseGoalPower<HoshinoAiPhase2EnergyGoalPower>(spent, ownerCreature);
                    await TryAdvancePhase();
                }

                return;
            case BossPhase.Phase3 when cardPlay.Card is Rebirth && _phase3RebirthPlayedPlayers.Add(ownerCreature):
                await DecreaseGoalPower<HoshinoAiPhase3RebirthGoalPower>(1, ownerCreature);
                await TryAdvancePhase();
                return;
            default:
                return;
        }
    }

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (_phase != BossPhase.Phase2 || amount <= 0 || creature.Player == null)
        {
            return;
        }

        await DecreaseGoalPower<HoshinoAiPhase2BlockGoalPower>((int)amount, creature);
        await TryAdvancePhase();
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature == Creature && !_killedByPlayerHookTriggered && delta < 0 && creature.CurrentHp <= 0)
        {
            _killedByPlayerHookTriggered = true;
            OnHoshinoAiKilledByPlayer();
        }

        return Task.CompletedTask;
    }

    private async Task TryAdvancePhase()
    {
        if (_phase == BossPhase.Phase1
            && GetGoalAmount<HoshinoAiPhase1DrawGoalPower>() <= 0
            && GetGoalAmount<HoshinoAiPhase1PlayGoalPower>() <= 0)
        {
            _phase = BossPhase.Phase2;
            _phase2UseOrbit = true;
            await EnterPhase2(GetPlayerCreatures().Count);
            return;
        }

        if (_phase == BossPhase.Phase2
            && GetGoalAmount<HoshinoAiPhase2BlockGoalPower>() <= 0
            && GetGoalAmount<HoshinoAiPhase2EnergyGoalPower>() <= 0)
        {
            _phase = BossPhase.Phase3;
            _phase3UseRebirthCard = true;
            _phase3RebirthPlayedPlayers.Clear();
            await EnterPhase3(GetPlayerCreatures().Count);
            return;
        }

        if (_phase == BossPhase.Phase3 && GetGoalAmount<HoshinoAiPhase3RebirthGoalPower>() <= 0)
        {
            if (Creature.CombatState?.RoundNumber <= 7)
            {
                OnPlayersCompletedAllTasksWithinSevenTurns();
            }

            _phase = BossPhase.Complete;
            _killedByPlayerHookTriggered = true;
            await CreatureCmd.Kill(Creature, force: true);
        }
    }

    private async Task Phase1GiveEnergyMove()
    {
        _phase1UseEnergy = false;
        await ApplyPowerToAllPlayers<EnergyNextTurnPower>(1);
    }

    private async Task Phase1GiveDrawMove()
    {
        _phase1UseEnergy = true;
        await ApplyPowerToAllPlayers<DrawCardsNextTurnPower>(1);
    }

    private async Task Phase2GiveOrbitMove()
    {
        _phase2UseOrbit = false;
        await ApplyPowerToAllPlayers<OrbitPower>(1);
    }

    private async Task Phase2GiveStatsMove()
    {
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await PowerCmd.Apply<StrengthPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
            await PowerCmd.Apply<DexterityPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
        }
    }

    private async Task Phase3GiveRebirthMove()
    {
        _phase3UseRebirthCard = false;

        var combatState = Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        foreach (var playerCreature in GetPlayerCreatures())
        {
            if (playerCreature.Player == null)
            {
                continue;
            }

            var rebirth = combatState.CreateCard<Rebirth>(playerCreature.Player);
            await CardPileCmd.AddGeneratedCardToCombat(rebirth, PileType.Discard, playerCreature.Player, CardPilePosition.Top);
        }
    }

    private async Task Phase3NoDrawMove()
    {
        foreach (var playerCreature in GetPlayerCreatures())
        {
            if (playerCreature.GetPower<HoshinoAiScrutinyPower>() == null)
            {
                await PowerCmd.Apply<HoshinoAiScrutinyPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
            }
        }
    }

    private async Task EnterPhase1(int playerCount)
    {
        PhaseChanged?.Invoke(this, 1);

        await RemovePhaseGoals(
            Creature.GetPower<HoshinoAiPhase1DrawGoalPower>(),
            Creature.GetPower<HoshinoAiPhase1PlayGoalPower>(),
            Creature.GetPower<HoshinoAiPhase2BlockGoalPower>(),
            Creature.GetPower<HoshinoAiPhase2EnergyGoalPower>(),
            Creature.GetPower<HoshinoAiPhase3RebirthGoalPower>()
        );

        await PowerCmd.Apply<HoshinoAiPhase1DrawGoalPower>(new BlockingPlayerChoiceContext(), Creature, 15 * Math.Max(1, playerCount), Creature, null, true);
        await PowerCmd.Apply<HoshinoAiPhase1PlayGoalPower>(new BlockingPlayerChoiceContext(), Creature, 16 * Math.Max(1, playerCount), Creature, null, true);
    }

    private async Task EnterPhase2(int playerCount)
    {
        PhaseChanged?.Invoke(this, 2);

        await RemovePhaseGoals(
            Creature.GetPower<HoshinoAiPhase1DrawGoalPower>(),
            Creature.GetPower<HoshinoAiPhase1PlayGoalPower>()
        );

        await PowerCmd.Apply<HoshinoAiPhase2BlockGoalPower>(new BlockingPlayerChoiceContext(), Creature, 50 * Math.Max(1, playerCount), Creature, null, true);
        await PowerCmd.Apply<HoshinoAiPhase2EnergyGoalPower>(new BlockingPlayerChoiceContext(), Creature, 14 * Math.Max(1, playerCount), Creature, null, true);
    }

    private async Task EnterPhase3(int playerCount)
    {
        PhaseChanged?.Invoke(this, 3);

        await RemovePhaseGoals(
            Creature.GetPower<HoshinoAiPhase2BlockGoalPower>(),
            Creature.GetPower<HoshinoAiPhase2EnergyGoalPower>()
        );

        await PowerCmd.Apply<HoshinoAiPhase3RebirthGoalPower>(new BlockingPlayerChoiceContext(), Creature, Math.Max(1, playerCount), Creature, null, true);
        ForcePhase3RebirthIntent();
    }

    private void ForcePhase3RebirthIntent()
    {
        var move = MoveStateMachine?.States.TryGetValue("PHASE3_REBIRTH_CARD", out var state) == true ? state as MoveState : null;
        if (move != null)
        {
            SetMoveImmediate(move, forceTransition: true);
        }
    }

    private async Task RemovePhaseGoals(params PowerModel?[] powers)
    {
        foreach (var power in powers)
        {
            if (power != null)
            {
                await PowerCmd.Remove(power);
            }
        }
    }

    private int GetGoalAmount<TPower>() where TPower : PowerModel
    {
        return Creature.GetPower<TPower>()?.Amount ?? 0;
    }

    private async Task DecreaseGoalPower<TPower>(int amount, Creature applier) where TPower : PowerModel
    {
        if (amount <= 0)
        {
            return;
        }

        var power = Creature.GetPower<TPower>();
        if (power == null || power.Amount <= 0)
        {
            return;
        }

        var offset = -Math.Min(power.Amount, amount);
        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, offset, applier, null, true);
    }

    private List<Creature> GetPlayerCreatures()
    {
        var combatState = Creature.CombatState;
        return combatState == null
            ? []
            : combatState.Players
            .Select(p => p.Creature)
            .Where(c => c != null)
            .Select(c => c!)
            .ToList();
    }

    private int GetPlayerCountForIntent()
    {
        try
        {
            return Math.Max(1, Creature.CombatState?.Players.Count ?? 1);
        }
        catch (InvalidOperationException)
        {
            // Intents are queried during preload before Creature is bound.
            return 1;
        }
    }

    private async Task ApplyPowerToAllPlayers<TPower>(int amount) where TPower : PowerModel
    {
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await PowerCmd.Apply<TPower>(new BlockingPlayerChoiceContext(), playerCreature, amount, Creature, null, true);
        }
    }

    private void OnHoshinoAiKilledByPlayer()
    {
        TaskHelper.RunSafely(GrantRelicAndPlayVideoForAllPlayers<BeHatred>("res://Oshinogo/videos/BeHatred.ogv"));
    }

    private void OnPlayersCompletedAllTasksWithinSevenTurns()
    {
        if (_completedAllTasksRewardIssued)
        {
            return;
        }

        _completedAllTasksRewardIssued = true;
        TaskHelper.RunSafely(GrantRelicAndPlayVideoForAllPlayers<BeHoped>("res://Oshinogo/videos/BeHoped.ogv"));
    }

    private async Task GrantRelicAndPlayVideoForAllPlayers<TRelic>(string videoPath) where TRelic : RelicModel
    {
        HoshinoAiBackgroundPatch.TryPlayFullscreenVideo(videoPath);

        var combatState = Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        foreach (var player in combatState.Players)
        {
            if (player.Relics.Any(r => r is TRelic))
            {
                continue;
            }

            await RelicCmd.Obtain<TRelic>(player);
        }
    }

}
