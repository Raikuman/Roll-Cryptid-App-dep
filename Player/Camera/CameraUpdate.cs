using Godot;
using System;

public partial class CameraUpdate : Node3D
{
	private Camera3D cameraObj;
	
	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Convert.ToInt32(GetParent().Name));
		((Camera3D)GetNode("Camera3D")).Current = IsMultiplayerAuthority();
	}
	
	public override void _Ready()
	{
		cameraObj = (Camera3D)GetNode("Camera3D");
		Position = new Vector3(0, 10, 0);
	}

	public override void _Input(InputEvent @event)
	{
		
	}
	
	public override void _Process(double delta)
	{
	}
}
