using Godot;
using System;

public partial class ChatHandler : Node
{
	private Control _chatContainer;
	
	public override void _Ready()
	{
		_chatContainer = GetParent().GetNode<PlayerUI>("PlayerUI").GetChatContainer();
	}

	public void CreateLog(string text, string username = "")
	{
		Rpc("_create_log", text, username);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _create_log(string text, string username = "")
	{
		var output = "[" + DateTime.Now.ToString("h:mm:ss") + "]";

		if (!string.IsNullOrEmpty(username))
		{
			output += " " + username + ": ";
		}

		output += " " + text;
		var log = ResourceLoader.Load<PackedScene>("res://Chat/ChatLog.tscn").Instantiate();
		log.GetNode<Label>("Label").Text = output;
		_chatContainer.AddChild(log);
	}
}
