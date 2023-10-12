using Godot;
using System;

public partial class ChatHideButton : Node
{
	private AnimateToScale _animatorScale;
	private AnimateToLocation _animatorLocation;
	private Label _buttonLabel;

	public override void _Ready()
	{
		_animatorScale = GetParent().GetParent().GetNode<AnimateToScale>("AnimateToScale");
		_animatorLocation = GetParent().GetParent().GetNode<AnimateToLocation>("AnimateToLocation");
		_buttonLabel = GetParent().GetNode<Label>("Label");
	}

	private void _on_button_pressed()
	{
		_animatorScale?.Animate();
		_animatorLocation?.Animate();

		if (_animatorLocation == null) return;

		if (_animatorLocation.IsOnTarget())
		{
			_buttonLabel.Text = "Open Chat";
		}
		else
		{
			_buttonLabel.Text = "Close Chat";
		}
	}
}
