using Godot;
using System;

public partial class AnimateToScale : Node
{
	[Export] private Control _control;
	[Export] private Vector2 _targetScale;
	[Export] private bool _lockWhenAnimating, _startAtTarget;
	[Export] private int _animationSpeed = 7;
	private Vector2 _originalScale, _animateScale;
	private bool _onTarget, _animate;
	
	
	public override void _Ready()
	{
		_originalScale = _control.Scale;

		if (_startAtTarget)
		{
			_control.Position = _targetScale;
			_animateScale = _targetScale;
			_onTarget = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_animate) return;

		_control.Scale = _control.Scale.Lerp(_animateScale, (float)delta * _animationSpeed);
	}

	public void Animate()
	{
		if (!_lockWhenAnimating || _lockWhenAnimating && !_animate)
		{
			_onTarget = !_onTarget;
			_animate = true;
			if (_onTarget)
			{
				_animateScale = _targetScale;
			}
			else
			{
				_animateScale = _originalScale;
			}
		}
	}

	public bool IsOnTarget()
	{
		return _onTarget;
	}
}
