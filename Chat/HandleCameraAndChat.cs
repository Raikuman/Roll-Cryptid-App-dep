using Godot;
using System;

public partial class HandleCameraAndChat : LineEdit
{
	private PlayerVariables _playerVariables;
	private Vector2 _topLeft, _bottomRight;
	private bool _entered, _enteredPrev;

	public override void _Ready()
	{
		_playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		UpdateArea();
		Resized += UpdateArea;
		FocusEntered += DisableCamera;
		TextSubmitted += SubmitText;
	}

	public override void _PhysicsProcess(double delta)
	{
		var mousePos = GetLocalMousePosition();
		
		if (mousePos.X > _topLeft.X && mousePos.X < _bottomRight.X && mousePos.Y > _topLeft.Y && mousePos.Y < _bottomRight.Y)
		{
			_entered = true;
		}
		else
		{
			_entered = false;
		}

		
		if (!_entered) LoseFocus();
		
		_enteredPrev = _entered;
	}

	private void UpdateArea()
	{
		_topLeft = Position;
		_bottomRight = Position + Size;
	}
	
	private void LoseFocus()
	{
		if (Input.IsActionJustPressed("lmb") || Input.IsActionJustPressed("rmb"))
		{
			ReleaseFocus();
			_playerVariables.CameraEnable(true);
		}
	}

	private void DisableCamera()
	{
		_playerVariables.CameraEnable(false);
	}

	private void SubmitText(string text)
	{
		if (string.IsNullOrEmpty(text)) return;
		
		GetNode<ChatHandler>("/root/Game/ChatHandler").CreateLog(text, _playerVariables.GetUsername());
		Text = "";
		ReleaseFocus();
		_playerVariables.CameraEnable(true);
	}

	private void _on_chat_button_pressed()
	{
		SubmitText(Text);
	}
}
