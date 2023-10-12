using Godot;
using System;

public partial class AnimateToLocation : Node
{
	[Export] private Control _control;
	[Export] private Vector2 _addPosition;
	[Export] private bool _lockWhenAnimating, _startAtTarget;
	[Export] private int _animationSpeed = 7;
	private Vector2 _originalPosition, _animatePosition;
	private bool _onTarget, _animate;
	
	public override void _Ready()
	{
		_originalPosition = _control.Position;

		if (_startAtTarget)
		{
			_control.Position = _originalPosition + _addPosition;
			_animatePosition = _originalPosition + _addPosition;
			_onTarget = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_animate) return;

		_control.Position = _control.Position.Lerp(_animatePosition, (float)delta * _animationSpeed);
	}

	public void Animate()
	{
		if (!_lockWhenAnimating || _lockWhenAnimating && !_animate)
		{
			_onTarget = !_onTarget;
			_animate = true;
			if (_onTarget)
			{
				_animatePosition = _originalPosition + _addPosition;
			}
			else
			{
				_animatePosition = _originalPosition;
			}
		}
	}

	public bool IsOnTarget()
	{
		return _onTarget;
	}
}
