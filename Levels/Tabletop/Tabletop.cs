using Godot;
using System;

public partial class Tabletop : Node3D
{
	public override void _ExitTree()
	{
		if (!Multiplayer.IsServer()) return;

		Multiplayer.PeerConnected -= AddPlayer;
		Multiplayer.PeerDisconnected -= DeletePlayer;
	}

	public override void _Ready()
	{
		if (!Multiplayer.IsServer()) return;
		
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
		if (!IsMultiplayerAuthority()) return;
		
		GD.Print("Disconnected: " + playerId);
		GetNode("Players/" + playerId).QueueFree();
	}
}
