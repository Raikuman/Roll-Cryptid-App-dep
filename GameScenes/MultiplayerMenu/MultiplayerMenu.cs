using Godot;
using System;

public partial class MultiplayerMenu : Node
{
	[ExportGroup("Buttons")]
	[Export] private Button _connectButton, _hostButton;

	[ExportGroup("TextLine")] 
	[Export] private LineEdit _usernameLine, _addressLine;
	
	[ExportGroup("Label")] 
	[Export] private Label _errorLabel;
	
	private MultiplayerVariables _multiplayerVariables;
	private PlayerVariables _playerVariables;
	private bool _isConnecting;

	public override void _Ready()
	{
		_multiplayerVariables = GetNode<MultiplayerVariables>("/root/MultiplayerVariables");
		_playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");

		if (_multiplayerVariables.GetConnection() == ConnectionEnum.FAILED)
		{
			if (_multiplayerVariables.GetHost())
			{
				_errorLabel.Text = "Failed to start multiplayer server";
			}
			else
			{
				_errorLabel.Text = "Failed to start multiplayer client";
			}
			
			((Fade)GetNode("Fade")).FadeOut();
		}
		
		_multiplayerVariables.Reset();
	}
	
	private void _on_connect_pressed()
	{
		_playerVariables.SetUsername(_usernameLine.Text);
		_multiplayerVariables.SetAddress(_addressLine.Text);
		((Fade)GetNode("Fade")).FadeIn();
		_isConnecting = true;
	}
	
	private void _on_host_pressed()
	{
		_playerVariables.SetUsername(_usernameLine.Text);
		_multiplayerVariables.SetAddress(_addressLine.Text);
		_multiplayerVariables.SetHost(true);
		((Fade)GetNode("Fade")).FadeIn();
		_isConnecting = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isConnecting) return;

		if (((Fade)GetNode("Fade")).AllowFunction())
		{
			//((Fade)GetNode("Fade")).Hide();
			GetTree().ChangeSceneToFile("res://GameScenes/Game/Game.tscn");
		}
	}
}
