using Godot;
using System;

public partial class PlayerUI : Node
{
	private Control _chatContainer;

	public override void _Ready()
	{
		_chatContainer = GetNode<Control>("Chat/Control/ChatBox/ChatContainer/ScrollContainer/VBoxContainer");
	}

	public Control GetChatContainer()
	{
		return _chatContainer;
	}
}
