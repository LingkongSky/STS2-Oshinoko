using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Oshinoko.Scripts.UI;

public partial class OshinokoNCreatureVisuals : NCreatureVisuals
{
	[Export] public float DeathFallAngleDeg { get; set; } = 90f;
	[Export] public double DeathFallDuration { get; set; } = 0.28d;

	private Node2D? _deathPivot;
	private Tween? _deathTween;
    private readonly List<AnimationPlayer> _deathAnimationPlayers = [];

	public override void _Ready()
	{
		base._Ready();
		_deathPivot = GetNodeOrNull<Node2D>("%DeathPivot");
        CacheDeathAnimationPlayers();
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
        _deathTween.Finished += PauseDeathAnimationPlayers;
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

        ResumeDeathAnimationPlayers();
	}

    private void CacheDeathAnimationPlayers()
    {
        _deathAnimationPlayers.Clear();
        if (_deathPivot == null || !GodotObject.IsInstanceValid(_deathPivot))
        {
            return;
        }

        foreach (var animationPlayer in FindAnimationPlayers(_deathPivot))
        {
            _deathAnimationPlayers.Add(animationPlayer);
        }
    }

    private static IEnumerable<AnimationPlayer> FindAnimationPlayers(Node root)
    {
        if (root is AnimationPlayer animationPlayer)
        {
            yield return animationPlayer;
        }

        foreach (Node child in root.GetChildren())
        {
            foreach (var nested in FindAnimationPlayers(child))
            {
                yield return nested;
            }
        }
    }

    private void PauseDeathAnimationPlayers()
    {
        foreach (var animationPlayer in _deathAnimationPlayers)
        {
            if (!GodotObject.IsInstanceValid(animationPlayer))
            {
                continue;
            }

            animationPlayer.Pause();
        }
    }

    private void ResumeDeathAnimationPlayers()
    {
        foreach (var animationPlayer in _deathAnimationPlayers)
        {
            if (!GodotObject.IsInstanceValid(animationPlayer))
            {
                continue;
            }

            var currentAnimation = animationPlayer.CurrentAnimation;
            if (string.IsNullOrEmpty(currentAnimation))
            {
                continue;
            }

            animationPlayer.Play(currentAnimation);
        }
    }
}
