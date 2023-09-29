using Godot;
using System;

public partial class MainMenu : Node
{
	[ExportGroup("Buttons")]
	[Export] private Button multiplayerButton;
	
	public override void _Process(double delta)
	{
	}

	private void _on_multiplayer_pressed()
	{
		GetTree().ChangeSceneToFile("res://GameScenes/MultiplayerMenu/MultiplayerMenu.tscn");
	}
}
