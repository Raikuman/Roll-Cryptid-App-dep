using Godot;
using System;

public partial class PlayerJoin : Node
{
	public override void _Ready()
	{
		SetMultiplayerAuthority(Convert.ToInt32(GetParent().Name));

		if (IsMultiplayerAuthority())
		{
			GetNode<ChatHandler>("/root/Game/ChatHandler").CreateLog(
				GetNode<PlayerVariables>("/root/PlayerVariables").GetUsername() + " has connected");
		}
	}
}
