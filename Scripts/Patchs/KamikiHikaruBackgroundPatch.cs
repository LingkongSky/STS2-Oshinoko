using Oshinogo.Scripts.Encounters;
using Oshinogo.Scripts.Monsters;

namespace Oshinogo.Scripts.Patchs;

public static class KamikiHikaruBackgroundPatch
{
    private const string OverlayName = "Oshinogo_KamikiPhaseBackgroundOverlay";
    private const string Phase1Path = "res://Oshinogo/images/backgrounds/hikaru_phase1.jpg";
    private const string Phase2Path = "res://Oshinogo/images/backgrounds/hikaru_phase2.jpg";

    private static WeakReference<object>? _currentCombatRoom;
    private static bool _initialized;
    private static readonly SemaphoreSlim TransitionLock = new(1, 1);

    public static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        KamikiHikaru.PhaseChanged += OnPhaseChanged;
        _initialized = true;
    }

    private static void OnPhaseChanged(KamikiHikaru _, int phase)
    {
        TaskHelper.RunSafely(HandlePhaseChangedAsync(phase));
    }

    private static async Task HandlePhaseChangedAsync(int phase)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state?.Encounter is not KamikiHikaruEncounter)
        {
            return;
        }

        await TransitionLock.WaitAsync();
        try
        {
            var room = ResolveCurrentCombatRoom();
            if (room == null)
            {
                return;
            }

            if (phase <= 1)
            {
                ApplyBackgroundForPhase(room, phase);
                ApplyPhaseVisuals(room, phase);
                return;
            }

            await PlayBlackTransitionAndSwap(room, phase);
        }
        finally
        {
            TransitionLock.Release();
        }
    }

    private static async Task PlayBlackTransitionAndSwap(object combatRoom, int phase)
    {
        if (combatRoom is not Control roomControl)
        {
            ApplyBackgroundForPhase(combatRoom, phase);
            return;
        }

        var fade = new ColorRect { Name = "Oshinogo_KamikiPhaseFadeOverlay", Color = new Color(0f, 0f, 0f, 0f) };
        ConfigureFullscreenOverlay(fade);
        fade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        roomControl.AddChild(fade);

        try
        {
            await TweenAlpha(fade, "color:a", 0.9f, 0.35);
            ApplyPhaseVisuals(combatRoom, phase);
            ApplyBackgroundForPhase(combatRoom, phase);
            await TweenAlpha(fade, "color:a", 0f, 0.45);
        }
        finally
        {
            if (GodotObject.IsInstanceValid(fade))
            {
                fade.QueueFree();
            }
        }
    }

    private static void ApplyBackgroundForPhase(object combatRoom, int phase)
    {
        string? imagePath = phase switch
        {
            1 => Phase1Path,
            2 => Phase2Path,
            _ => null,
        };

        if (imagePath == null)
        {
            return;
        }

        var backgroundNode = GetCombatBackgroundNode(combatRoom);
        if (backgroundNode == null)
        {
            return;
        }

        var overlay = backgroundNode.GetNodeOrNull<TextureRect>(OverlayName);
        if (overlay == null)
        {
            overlay = new TextureRect
            {
                Name = OverlayName,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            };
            backgroundNode.AddChild(overlay);
        }

        backgroundNode.MoveChild(overlay, backgroundNode.GetChildCount() - 1);

        var tex = ResourceLoader.Load<Texture2D>(imagePath);
        if (tex == null)
        {
            GD.PushWarning($"Failed to load Kamiki phase background: {imagePath}");
            return;
        }

        overlay.Texture = tex;
        LayoutOverlayToViewport(backgroundNode, overlay);
    }

    private static void LayoutOverlayToViewport(CanvasItem backgroundNode, TextureRect overlay)
    {
        var viewport = backgroundNode.GetViewport();
        if (viewport == null)
        {
            return;
        }

        var viewportSize = viewport.GetVisibleRect().Size;
        var toLocal = backgroundNode.GetGlobalTransformWithCanvas().AffineInverse();
        const float overscanScale = 1.10f;
        var overscanPadding = viewportSize * ((overscanScale - 1f) * 0.5f);

        var expandedTopLeftScreen = -overscanPadding;
        var expandedBottomRightScreen = viewportSize + overscanPadding;

        var topLeft = toLocal * expandedTopLeftScreen;
        var bottomRight = toLocal * expandedBottomRightScreen;

        overlay.Position = topLeft;
        overlay.Size = bottomRight - topLeft;
    }

    private static object? ResolveCurrentCombatRoom()
    {
        if (_currentCombatRoom != null && _currentCombatRoom.TryGetTarget(out var cachedRoom))
        {
            return cachedRoom;
        }

        var resolved = TryResolveCombatRoomFromSceneTree();
        if (resolved != null)
        {
            _currentCombatRoom = new WeakReference<object>(resolved);
        }

        return resolved;
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

    private static CanvasItem? GetCombatBackgroundNode(object combatRoom)
    {
        return combatRoom.GetType().GetProperty("Background")?.GetValue(combatRoom) as CanvasItem;
    }

    private static void ApplyPhaseVisuals(object combatRoom, int phase)
    {
        if (combatRoom is not Node roomNode)
        {
            return;
        }

        var showPhase2 = phase >= 2;
        SetVisualNodeVisible(roomNode, "Hikaru1", !showPhase2);
        SetVisualNodeVisible(roomNode, "Hikaru2", showPhase2);
        PlayHikaruIdleAnimation(roomNode, showPhase2 ? "HikaruIdle2" : "HikaruIdle1");
    }

    private static void PlayHikaruIdleAnimation(Node root, string animationName)
    {
        foreach (var player in FindAnimationPlayers(root))
        {
            if (player.HasAnimation(animationName))
            {
                player.Play(animationName);
            }
        }
    }

    private static void SetVisualNodeVisible(Node root, string nodeName, bool visible)
    {
        foreach (var node in FindNodesByName(root, nodeName))
        {
            if (node is CanvasItem canvasItem)
            {
                canvasItem.Visible = visible;
            }
        }
    }

    private static IEnumerable<Node> FindNodesByName(Node root, string name)
    {
        if (root.Name == name)
        {
            yield return root;
        }

        foreach (Node child in root.GetChildren())
        {
            foreach (var match in FindNodesByName(child, name))
            {
                yield return match;
            }
        }
    }

    private static IEnumerable<AnimationPlayer> FindAnimationPlayers(Node root)
    {
        if (root is AnimationPlayer player)
        {
            yield return player;
        }

        foreach (Node child in root.GetChildren())
        {
            foreach (var nested in FindAnimationPlayers(child))
            {
                yield return nested;
            }
        }
    }

    private static void ConfigureFullscreenOverlay(Control control)
    {
        control.TopLevel = true;
        control.ZIndex = 999;
        control.MouseFilter = Control.MouseFilterEnum.Ignore;
    }

    private static async Task TweenAlpha(CanvasItem node, string property, float alpha, double duration)
    {
        var tween = node.CreateTween();
        tween.TweenProperty(node, property, alpha, duration);
        await node.ToSignal(tween, Tween.SignalName.Finished);
    }
}
