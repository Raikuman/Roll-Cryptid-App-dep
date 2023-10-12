using Godot;
using System;

public partial class HandleGameMenu : Node
{
	private PlayerVariables _playerVariables;

	public override void _Ready()
	{
		_playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
	}

	private void _menu_button_pressed()
	{
		GetNode<Control>("Menu").Show();
		_playerVariables.PauseUI(true);
	}

	private void _resume_button_pressed()
	{
		GetNode<Control>("Menu").Hide();
		_playerVariables.PauseUI(false);
	}

	private void _exit_button_pressed()
	{
		if (!Multiplayer.IsServer())
		{
			Rpc("_disconnect", Multiplayer.MultiplayerPeer.GetUniqueId());
		}
		else
		{
			foreach (var peerId in Multiplayer.GetPeers())
			{
				Multiplayer.MultiplayerPeer.DisconnectPeer(peerId);
			}
			Multiplayer.MultiplayerPeer.Close();
		}
		
		GetTree().ChangeSceneToFile("res://GameScenes/MultiplayerMenu/MultiplayerMenu.tscn");
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void _disconnect(int playerId)
	{
		if (Multiplayer.IsServer())
		{
			Multiplayer.MultiplayerPeer.DisconnectPeer(playerId);
		}
	}
}
