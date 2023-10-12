using Godot;
using System;

public partial class MultiplayerHandler : Node
{
	[Export] private Node levelNode;
	
	private MultiplayerVariables multiplayerVariables;

	private bool connecting, isLoading;
	private double elapsedTime, timeout = 5.0;
	
	public override void _ExitTree()
	{
		if (!Multiplayer.IsServer()) return;

		Multiplayer.PeerConnected -= AddPlayer;
		Multiplayer.PeerDisconnected -= DeletePlayer;
	}
	
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
				ChangeLevel(ResourceLoader.Load<PackedScene>("res://Levels/Tabletop/Tabletop.tscn"));
				((Fade)GetNode("Fade")).FadeIn();
				if (Multiplayer.IsServer()) HandleMultiplayerEvents();
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

	private void HandleMultiplayerEvents()
	{
		Multiplayer.PeerConnected += AddPlayer;
		Multiplayer.PeerDisconnected += DeletePlayer;
		
		// Spawn already connected players
		foreach (var peerId in Multiplayer.GetPeers())
		{
			AddPlayer(peerId);
		}
		
		// Spawn local player
		AddPlayer();
	}
	
	private void AddPlayer(long playerId = 1)
	{
		GD.Print("Connected: " + playerId);
		var player = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn").Instantiate();
		player.Name = playerId.ToString();
		GetNode("Players").AddChild(player);
	}
	
	private void DeletePlayer(long playerId)
	{
		Rpc("_delete_player", playerId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _delete_player(long playerId)
	{
		GD.Print("Disconnected: " + playerId);
		GetNode("Players/" + playerId).QueueFree();
		GetNode("Dice/" + playerId).QueueFree();
	}
}
