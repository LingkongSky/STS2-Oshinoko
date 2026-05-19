using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Oshinogo.Scripts.Cards.Colorless;
using Oshinogo.Scripts.Encounters;

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
    private readonly HashSet<Creature> _phase3RebirthPlayedPlayers = new();

    public static event Action<HoshinoAi, int>? PhaseChanged;

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 255, 255);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 255, 255);

    public override MonsterAssetProfile AssetProfile => new(
    VisualsScenePath: "res://Oshinogo/scenes/monster/ai.tscn"
    );

    public override async Task AfterAddedToRoom()
    {
        HoshinoAiBackgroundPatch.EnsureInitialized();

        var players = GetPlayerCreatures();
        var playerCount = players.Count;

        foreach (var playerCreature in players)
        {
            await PowerCmd.Apply<PassionPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
        }

        await PowerCmd.Apply<HoshinoAiMechanicsPower>(new BlockingPlayerChoiceContext(), Creature, 1, Creature, null, true);
        await EnterPhase1(playerCount);
    }



    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var phase1Energy = new MoveState("PHASE1_ENERGY", async _ => await Phase1GiveEnergyMove(), new BuffIntent());
        var phase1Draw = new MoveState("PHASE1_DRAW", async _ => await Phase1GiveDrawMove(), new BuffIntent());
        var phase2Orbit = new MoveState("PHASE2_ORBIT", async _ => await Phase2GiveOrbitMove(), new BuffIntent());
        var phase2Stats = new MoveState("PHASE2_STATS", async _ => await Phase2GiveStatsMove(), new BuffIntent());
        var phase3Rebirth = new MoveState("PHASE3_REBIRTH_CARD", async _ => await Phase3GiveRebirthMove(), new StatusIntent(1));
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
        if (_phase != BossPhase.Phase1)
        {
            return;
        }

        if (card.Owner?.Creature?.Player == null)
        {
            return;
        }

        await DecreaseGoalPower<HoshinoAiPhase1DrawGoalPower>(1, card.Owner.Creature);
        TryAdvancePhase();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var ownerCreature = cardPlay.Card.Owner?.Creature;
        if (ownerCreature?.Player == null)
        {
            return;
        }

        if (_phase == BossPhase.Phase1)
        {
            await DecreaseGoalPower<HoshinoAiPhase1PlayGoalPower>(1, ownerCreature);
            TryAdvancePhase();
            return;
        }

        if (_phase == BossPhase.Phase2)
        {
            var spent = (int)Math.Max(0, cardPlay.Card.EnergyCost.GetWithModifiers(CostModifiers.All));
            if (spent > 0)
            {
                await DecreaseGoalPower<HoshinoAiPhase2EnergyGoalPower>(spent, ownerCreature);
                TryAdvancePhase();
            }

            return;
        }

        if (_phase == BossPhase.Phase3 && cardPlay.Card is Rebirth)
        {
            if (_phase3RebirthPlayedPlayers.Add(ownerCreature))
            {
                await DecreaseGoalPower<HoshinoAiPhase3RebirthGoalPower>(1, ownerCreature);
                TryAdvancePhase();
            }
        }
    }

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (_phase != BossPhase.Phase2 || amount <= 0 || creature.Player == null)
        {
            return;
        }

        await DecreaseGoalPower<HoshinoAiPhase2BlockGoalPower>((int)amount, creature);
        TryAdvancePhase();
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Creature || _killedByPlayerHookTriggered)
        {
            return;
        }

        if (result.WasTargetKilled)
        {

            //
        }

        if (result.WasTargetKilled && dealer?.Player != null && _phase != BossPhase.Complete)
        {
            _killedByPlayerHookTriggered = true;
            OnHoshinoAiKilledByPlayer();
        }

        await Task.CompletedTask;
    }

    private void TryAdvancePhase()
    {
        if (_phase == BossPhase.Phase1
            && GetGoalAmount<HoshinoAiPhase1DrawGoalPower>() <= 0
            && GetGoalAmount<HoshinoAiPhase1PlayGoalPower>() <= 0)
        {
            _phase = BossPhase.Phase2;
            _phase2UseOrbit = true;
            TaskHelper.RunSafely(EnterPhase2(GetPlayerCreatures().Count));
            return;
        }

        if (_phase == BossPhase.Phase2
            && GetGoalAmount<HoshinoAiPhase2BlockGoalPower>() <= 0
            && GetGoalAmount<HoshinoAiPhase2EnergyGoalPower>() <= 0)
        {
            _phase = BossPhase.Phase3;
            _phase3UseRebirthCard = true;
            _phase3RebirthPlayedPlayers.Clear();
            TaskHelper.RunSafely(EnterPhase3(GetPlayerCreatures().Count));
            return;
        }

        if (_phase == BossPhase.Phase3 && GetGoalAmount<HoshinoAiPhase3RebirthGoalPower>() <= 0)
        {
            if (Creature.CombatState?.RoundNumber <= 5)
            {
                OnPlayersCompletedAllTasksWithinFiveTurns();
            }

            _phase = BossPhase.Complete;
            TaskHelper.RunSafely(CreatureCmd.Kill(Creature, force: true));
        }
    }

    private async Task Phase1GiveEnergyMove()
    {
        _phase1UseEnergy = false;
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
        }
    }

    private async Task Phase1GiveDrawMove()
    {
        _phase1UseEnergy = true;
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await PowerCmd.Apply<DrawCardsNextTurnPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
        }
    }

    private async Task Phase2GiveOrbitMove()
    {
        _phase2UseOrbit = false;
        foreach (var playerCreature in GetPlayerCreatures())
        {
            await PowerCmd.Apply<OrbitPower>(new BlockingPlayerChoiceContext(), playerCreature, 1, Creature, null, true);
        }
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
        await PowerCmd.Apply<HoshinoAiPhase1PlayGoalPower>(new BlockingPlayerChoiceContext(), Creature, 15 * Math.Max(1, playerCount), Creature, null, true);
    }

    private async Task EnterPhase2(int playerCount)
    {
        PhaseChanged?.Invoke(this, 2);

        await RemovePhaseGoals(
            Creature.GetPower<HoshinoAiPhase1DrawGoalPower>(),
            Creature.GetPower<HoshinoAiPhase1PlayGoalPower>()
        );

        await PowerCmd.Apply<HoshinoAiPhase2BlockGoalPower>(new BlockingPlayerChoiceContext(), Creature, 40 * Math.Max(1, playerCount), Creature, null, true);
        await PowerCmd.Apply<HoshinoAiPhase2EnergyGoalPower>(new BlockingPlayerChoiceContext(), Creature, 10 * Math.Max(1, playerCount), Creature, null, true);
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
        if (combatState == null)
        {
            return [];
        }

        return combatState.Players.Select(p => p.Creature).Where(c => c != null).ToList()!;
    }

    private void OnHoshinoAiKilledByPlayer()
    {
    }

    private void OnPlayersCompletedAllTasksWithinFiveTurns()
    {
    }



}

public static class HoshinoAiBackgroundPatch
{
    private const string OverlayName = "Oshinogo_AiPhaseBackgroundOverlay";
    private const string Phase1Path = "res://Oshinogo/images/backgrounds/ai_phase1.jpg";
    private const string Phase2Path = "res://Oshinogo/images/backgrounds/ai_phase2.jpg";
    private const string Phase3Path = "res://Oshinogo/images/backgrounds/ai_phase3.jpg";

    private static WeakReference<object>? _currentAiCombatRoom;
    private static bool _initialized;

    static HoshinoAiBackgroundPatch()
    {
        EnsureInitialized();
    }

    public static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        HoshinoAi.PhaseChanged += OnPhaseChanged;
        _initialized = true;
    }

    private static void OnPhaseChanged(HoshinoAi _, int phase)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state?.Encounter is not AiEncounter)
        {
            return;
        }

        if (_currentAiCombatRoom == null || !_currentAiCombatRoom.TryGetTarget(out var room))
        {
            room = TryResolveCombatRoomFromSceneTree();
            if (room == null)
            {
                return;
            }

            _currentAiCombatRoom = new WeakReference<object>(room);
        }

        ApplyBackgroundForPhase(room, phase);
    }

    private static void ApplyBackgroundForPhase(object combatRoom, int phase)
    {
        string? imagePath = phase switch
        {
            1 => Phase1Path,
            2 => Phase2Path,
            3 => Phase3Path,
            _ => null,
        };

        if (imagePath == null)
        {
            return;
        }

        var backgroundNode = combatRoom.GetType().GetProperty("Background")?.GetValue(combatRoom) as CanvasItem;
        if (backgroundNode == null)
        {
            return;
        }

        var hostNode = backgroundNode;
        var overlay = hostNode.GetNodeOrNull<TextureRect>(OverlayName);
        if (overlay == null)
        {
            overlay = new TextureRect
            {
                Name = OverlayName,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            };
            hostNode.AddChild(overlay);
        }

        hostNode.MoveChild(overlay, hostNode.GetChildCount() - 1);

        var tex = ResourceLoader.Load<Texture2D>(imagePath);
        if (tex == null)
        {
            GD.PushWarning($"Failed to load AI phase background: {imagePath}");
            return;
        }

        overlay.Texture = tex;
        LayoutOverlayToViewport(hostNode, overlay);
    }

    private static void LayoutOverlayToViewport(CanvasItem backgroundNode, TextureRect overlay)
    {
        var viewport = backgroundNode.GetViewport();
        if (viewport == null)
        {
            return;
        }

        Vector2 viewportSize = viewport.GetVisibleRect().Size;
        Transform2D toLocal = backgroundNode.GetGlobalTransformWithCanvas().AffineInverse();
        const float overscanScale = 1.10f;
        Vector2 overscanPadding = viewportSize * ((overscanScale - 1f) * 0.5f);

        Vector2 expandedTopLeftScreen = -overscanPadding;
        Vector2 expandedBottomRightScreen = viewportSize + overscanPadding;

        Vector2 topLeft = toLocal * expandedTopLeftScreen;
        Vector2 bottomRight = toLocal * expandedBottomRightScreen;

        overlay.Position = topLeft;
        overlay.Size = bottomRight - topLeft;
    }

    private static object? TryResolveCombatRoomFromSceneTree()
    {
        if (Engine.GetMainLoop() is not SceneTree tree)
        {
            return null;
        }

        return FindCombatRoomNode(tree.Root);
    }

    private static object? FindCombatRoomNode(Node? node)
    {
        if (node == null)
        {
            return null;
        }

        var type = node.GetType();
        if (type.FullName == "MegaCrit.Sts2.Core.Nodes.Rooms.NCombatRoom" &&
            type.GetProperty("Background")?.GetValue(node) is CanvasItem)
        {
            return node;
        }

        foreach (Node child in node.GetChildren())
        {
            var found = FindCombatRoomNode(child);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}


