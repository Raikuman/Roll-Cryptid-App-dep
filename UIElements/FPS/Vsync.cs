using Godot;
using System;

public partial class Vsync : Node
{
	private void toggled(bool pressed)
	{
		if (pressed)
		{
			((Label)GetNode("Label2")).Text = "VSYNC: ON";
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		}
		else
		{
			((Label)GetNode("Label2")).Text = "VSYNC: OFF";
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		}
	}
}
