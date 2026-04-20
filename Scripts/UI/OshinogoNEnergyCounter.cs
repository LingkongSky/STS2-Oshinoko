using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Oshinogo.Scripts.UI;

public partial class OshinogoNEnergyCounter : NEnergyCounter
{
	[Export] public float RefillHighlightBoost = 0.75f;
	[Export] public double RefillHighlightInDuration = 0.12d;
	[Export] public double RefillHighlightOutDuration = 0.2d;

	private Control? _layers;
	private TextureRect? _layer1;
	private TextureRect? _highlightOverlay;
	private Tween? _highlightTween;
	private ulong _lastRefillHighlightAtMs;

	public override void _Ready()
	{
		base._Ready();
		_layers = GetNodeOrNull<Control>("Layers");
		_layer1 = GetNodeOrNull<TextureRect>("Layers/Layer1");
		EnsureHighlightOverlay();
	}

	public void PlayRoundStartRefillHighlight()
	{
		if ((_layers == null || !GodotObject.IsInstanceValid(_layers)) &&
			!TryResolveLayers())
		{
			return;
		}

		EnsureHighlightOverlay();
		if (_highlightOverlay == null || !GodotObject.IsInstanceValid(_highlightOverlay))
		{
			return;
		}

		ulong now = Time.GetTicksMsec();
		if (now - _lastRefillHighlightAtMs < 100)
		{
			return;
		}

		_lastRefillHighlightAtMs = now;

		if (_highlightTween != null && GodotObject.IsInstanceValid(_highlightTween))
		{
			_highlightTween.Kill();
		}

		var start = new Color(1f, 1f, 1f, 0f);
		var peak = new Color(1f, 1f, 1f, Mathf.Clamp(RefillHighlightBoost, 0f, 1.2f));
		_highlightOverlay.Modulate = start;

		_highlightTween = CreateTween();
		_highlightTween.TweenProperty(_highlightOverlay, "modulate", peak, RefillHighlightInDuration)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
		_highlightTween.TweenProperty(_highlightOverlay, "modulate", start, RefillHighlightOutDuration)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);
	}

	private bool TryResolveLayers()
	{
		_layers = GetNodeOrNull<Control>("Layers");
		_layer1 = GetNodeOrNull<TextureRect>("Layers/Layer1");
		return _layers != null;
	}

	private void EnsureHighlightOverlay()
	{
		if (_highlightOverlay != null && GodotObject.IsInstanceValid(_highlightOverlay))
		{
			return;
		}

		if (_layers == null || !GodotObject.IsInstanceValid(_layers))
		{
			return;
		}

		_highlightOverlay = new TextureRect
		{
			Name = "RefillHighlightOverlay",
			MouseFilter = Control.MouseFilterEnum.Ignore,
			AnchorsPreset = (int)Control.LayoutPreset.FullRect,
			AnchorRight = 1f,
			AnchorBottom = 1f,
			GrowHorizontal = Control.GrowDirection.Both,
			GrowVertical = Control.GrowDirection.Both,
			StretchMode = TextureRect.StretchModeEnum.Scale,
			ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional,
			Modulate = new Color(1f, 1f, 1f, 0f),
			Texture = _layer1?.Texture,
			ZIndex = 20
		};

		var addMat = ResourceLoader.Load<Material>("res://Oshinogo/themes/canvas_item_material_additive_shared.tres");
		if (addMat != null)
		{
			_highlightOverlay.Material = addMat;
		}

		_layers.AddChild(_highlightOverlay);
	}
}
