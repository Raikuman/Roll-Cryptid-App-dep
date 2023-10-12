using Godot;
using System;

public partial class HoverToAnimate : Control
{
	[Export] private Control _hoverArea;
	private AnimateToLocation _animator;
	private Vector2 _topLeft, _bottomRight;
	private bool _entered, _enteredPrev;
	private PlayerVariables _playerVariables;
	
	public override void _Ready()
	{
		_animator = GetParent().GetNode<AnimateToLocation>("AnimateToLocation");
		_playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		
		UpdateArea();
		_hoverArea.Resized += UpdateArea;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_playerVariables.GetPauseUI()) return;
		
		var mousePos = GetLocalMousePosition();

		if (mousePos.X > _topLeft.X && mousePos.X < _bottomRight.X && mousePos.Y > _topLeft.Y && mousePos.Y < _bottomRight.Y)
		{
			_entered = true;
		}
		else
		{
			_entered = false;
		}

		if (_entered != _enteredPrev) _animator?.Animate();
		_enteredPrev = _entered;
	}

	private void UpdateArea()
	{
		_topLeft = _hoverArea.Position;
		_bottomRight = _hoverArea.Position + _hoverArea.Size;
	}
}
