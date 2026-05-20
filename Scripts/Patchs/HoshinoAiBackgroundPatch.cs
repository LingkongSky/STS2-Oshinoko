using Oshinogo.Scripts.Encounters;
using Oshinogo.Scripts.Monsters;

namespace Oshinogo.Scripts.Patchs;

public static class HoshinoAiBackgroundPatch
{
    private const string OverlayName = "Oshinogo_AiPhaseBackgroundOverlay";
    private const string Phase1Path = "res://Oshinogo/images/backgrounds/ai_phase1.jpg";
    private const string Phase2Path = "res://Oshinogo/images/backgrounds/ai_phase2.jpg";
    private const string Phase3Path = "res://Oshinogo/images/backgrounds/ai_phase3.jpg";
    private const string FullscreenVideoOverlayName = "Oshinogo_AiFullscreenVideoOverlay";

    private static WeakReference<object>? _currentAiCombatRoom;
    private static bool _initialized;
    private static readonly SemaphoreSlim TransitionLock = new(1, 1);

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
        TaskHelper.RunSafely(HandlePhaseChangedAsync(phase));
    }

    private static async Task HandlePhaseChangedAsync(int phase)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state?.Encounter is not AiEncounter)
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
                return;
            }

            await PlayPinkTransitionAndSwap(room, phase);
        }
        finally
        {
            TransitionLock.Release();
        }
    }

    private static async Task PlayPinkTransitionAndSwap(object combatRoom, int phase)
    {
        if (combatRoom is not Control roomControl)
        {
            ApplyBackgroundForPhase(combatRoom, phase);
            return;
        }

        var fade = new ColorRect { Name = "Oshinogo_AiPhaseFadeOverlay", Color = new Color(1f, 0.52f, 0.72f, 0f) };
        ConfigureFullscreenOverlay(fade, Control.MouseFilterEnum.Ignore);
        fade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        roomControl.AddChild(fade);

        try
        {
            await TweenAlpha(fade, "color:a", 0.82f, 0.42);

            ApplyBackgroundForPhase(combatRoom, phase);

            await TweenAlpha(fade, "color:a", 0f, 0.48);
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
            3 => Phase3Path,
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
            GD.PushWarning($"Failed to load AI phase background: {imagePath}");
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

    private static object? ResolveCurrentCombatRoom()
    {
        if (_currentAiCombatRoom != null && _currentAiCombatRoom.TryGetTarget(out var cachedRoom))
        {
            return cachedRoom;
        }

        var resolved = TryResolveCombatRoomFromSceneTree();
        if (resolved != null)
        {
            _currentAiCombatRoom = new WeakReference<object>(resolved);
        }

        return resolved;
    }

    private static CanvasItem? GetCombatBackgroundNode(object combatRoom)
    {
        return combatRoom.GetType().GetProperty("Background")?.GetValue(combatRoom) as CanvasItem;
    }

    public static void TryPlayFullscreenVideo(string videoPath)
    {
        TaskHelper.RunSafely(PlayFullscreenVideoAsync(videoPath));
    }

    private static async Task PlayFullscreenVideoAsync(string videoPath)
    {
        var roomObj = TryResolveCombatRoomFromSceneTree();
        if (roomObj is not Control roomControl)
        {
            return;
        }

        var existing = roomControl.GetNodeOrNull<Control>(FullscreenVideoOverlayName);
        existing?.QueueFree();

        var overlay = new Control { Name = FullscreenVideoOverlayName };
        ConfigureFullscreenOverlay(overlay, Control.MouseFilterEnum.Stop);
        overlay.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        var videoPlayer = new VideoStreamPlayer
        {
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Expand = true,
            Autoplay = false,
        };
        videoPlayer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        var stream = ResourceLoader.Load<VideoStream>(videoPath);
        if (stream == null)
        {
            overlay.QueueFree();
            GD.PushWarning($"Failed to load video stream: {videoPath}");
            return;
        }

        videoPlayer.Stream = stream;
        overlay.AddChild(videoPlayer);
        roomControl.AddChild(overlay);

        bool closeRequested = false;
        void OnOverlayGuiInput(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseButton mouseButton &&
                mouseButton.Pressed &&
                mouseButton.ButtonIndex == MouseButton.Left)
            {
                closeRequested = true;
            }
        }

        overlay.GuiInput += OnOverlayGuiInput;
        overlay.Modulate = new Color(1f, 1f, 1f, 0f);
        videoPlayer.Play();

        try
        {
            await TweenAlpha(overlay, "modulate:a", 1f, 0.2);

            var tree = overlay.GetTree();
            while (GodotObject.IsInstanceValid(overlay) &&
                   GodotObject.IsInstanceValid(videoPlayer) &&
                   videoPlayer.IsPlaying() &&
                   !closeRequested)
            {
                if (tree == null)
                {
                    break;
                }

                await overlay.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
            }

            if (closeRequested && GodotObject.IsInstanceValid(videoPlayer) && videoPlayer.IsPlaying())
            {
                videoPlayer.Stop();
            }

            if (GodotObject.IsInstanceValid(overlay))
            {
                await TweenAlpha(overlay, "modulate:a", 0f, 0.2);
            }
        }
        finally
        {
            overlay.GuiInput -= OnOverlayGuiInput;
        }

        if (GodotObject.IsInstanceValid(overlay))
        {
            overlay.QueueFree();
        }
    }

    private static void ConfigureFullscreenOverlay(Control control, Control.MouseFilterEnum mouseFilter)
    {
        control.TopLevel = true;
        control.ZIndex = 999;
        control.MouseFilter = mouseFilter;
    }

    private static async Task TweenAlpha(CanvasItem node, string property, float alpha, double duration)
    {
        var tween = node.CreateTween();
        tween.TweenProperty(node, property, alpha, duration);
        await node.ToSignal(tween, Tween.SignalName.Finished);
    }
}
