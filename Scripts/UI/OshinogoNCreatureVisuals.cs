using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Oshinogo.Scripts.UI;

public partial class OshinogoNCreatureVisuals : NCreatureVisuals
{
	[Export] public float DeathFallAngleDeg { get; set; } = 90f;
	[Export] public double DeathFallDuration { get; set; } = 0.28d;

	private Node2D? _deathPivot;
	private Tween? _deathTween;

	public override void _Ready()
	{
		base._Ready();
		_deathPivot = GetNodeOrNull<Node2D>("%DeathPivot");
		ResetDeathFallState();
	}

	public void PlayDeathFallAnim()
	{
		if (_deathPivot == null || !GodotObject.IsInstanceValid(_deathPivot))
		{
			return;
		}

		if (_deathTween != null && GodotObject.IsInstanceValid(_deathTween))
		{
			_deathTween.Kill();
		}

		_deathPivot.RotationDegrees = 0f;
		_deathTween = CreateTween();
		_deathTween.TweenProperty(_deathPivot, "rotation_degrees", DeathFallAngleDeg, DeathFallDuration)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}

	public void ResetDeathFallState()
	{
		if (_deathTween != null && GodotObject.IsInstanceValid(_deathTween))
		{
			_deathTween.Kill();
		}

		if (_deathPivot != null && GodotObject.IsInstanceValid(_deathPivot))
		{
			_deathPivot.RotationDegrees = 0f;
		}
	}
}
