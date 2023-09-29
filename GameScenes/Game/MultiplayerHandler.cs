using Godot;
using System;

public partial class MultiplayerHandler : Node
{
	[Export] private Node levelNode;
	
	private MultiplayerVariables multiplayerVariables;

	private bool connecting, isLoading;
	private double elapsedTime, timeout = 5.0;
	
	public override void _Ready()
	{
		multiplayerVariables = GetNode<MultiplayerVariables>("/root/MultiplayerVariables");

		var peer = new ENetMultiplayerPeer();
		if (multiplayerVariables.GetHost())
		{
			peer.CreateServer(multiplayerVariables.GetPort());
			Multiplayer.MultiplayerPeer = peer;

			connecting = true;
		}
		else
		{
			peer.CreateClient(multiplayerVariables.GetAddress(), multiplayerVariables.GetPort());
			Multiplayer.MultiplayerPeer = peer;
			
			connecting = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (connecting)
		{
			if (Multiplayer.MultiplayerPeer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Connected)
			{
				// Timeout
				if (elapsedTime > timeout)
				{
					connecting = false;
					multiplayerVariables.SetConnection(ConnectionEnum.FAILED);
					GetTree().ChangeSceneToFile("res://GameScenes/MultiplayerMenu/MultiplayerMenu.tscn");
				}
			}
			else
			{
				connecting = false;
				((Label)GetNode("ConnectUI/ConnectLabel")).Text = "Connected!";
				ChangeLevel(ResourceLoader.Load<PackedScene>("res://Levels/Tabletop.tscn"));
				((Fade)GetNode("Fade")).FadeIn();
				isLoading = true;
			}
			
			elapsedTime += delta;
		}

		if (isLoading && ((Fade)GetNode("Fade")).AllowFunction())
		{
			((Fade)GetNode("Fade")).FadeOut();
			((Control)GetNode("ConnectUI")).Hide();
			isLoading = false;
		}
	}

	private void ChangeLevel(PackedScene scene)
	{
		if (!Multiplayer.IsServer()) return;
		
		foreach (var child in levelNode.GetChildren())
		{
			levelNode.RemoveChild(child);
			child.QueueFree();
		}
		
		levelNode.AddChild(scene.Instantiate());
	}
}
