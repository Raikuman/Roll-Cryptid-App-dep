using Godot;
using System;

public partial class MultiplayerMenu : Node
{
	[ExportGroup("Buttons")]
	[Export] private Button connectButton, hostButton;

	[ExportGroup("TextLine")] 
	[Export] private LineEdit addressLine;
	
	[ExportGroup("Label")] 
	[Export] private Label errorLabel;
	
	private MultiplayerVariables multiplayerVariables;
	private bool isConnecting;

	public override void _Ready()
	{
		multiplayerVariables = GetNode<MultiplayerVariables>("/root/MultiplayerVariables");

		if (multiplayerVariables.GetConnection() == ConnectionEnum.FAILED)
		{
			if (multiplayerVariables.GetHost())
			{
				errorLabel.Text = "Failed to start multiplayer server";
			}
			else
			{
				errorLabel.Text = "Failed to start multiplayer client";
			}
			
			((Fade)GetNode("Fade")).FadeOut();
		}
		
		multiplayerVariables.Reset();
	}
	
	private void _on_connect_pressed()
	{
		multiplayerVariables.SetAddress(addressLine.Text);
		((Fade)GetNode("Fade")).FadeIn();
		isConnecting = true;
	}
	
	private void _on_host_pressed()
	{
		multiplayerVariables.SetAddress(addressLine.Text);
		multiplayerVariables.SetHost(true);
		((Fade)GetNode("Fade")).FadeIn();
		isConnecting = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isConnecting) return;

		if (((Fade)GetNode("Fade")).AllowFunction())
		{
			//((Fade)GetNode("Fade")).Hide();
			GetTree().ChangeSceneToFile("res://GameScenes/Game/Game.tscn");
		}
	}
}
