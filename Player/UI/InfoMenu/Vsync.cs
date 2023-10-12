using Godot;
using System;

public partial class Vsync : CheckButton
{
	[Export] private Label _vsyncLabel;

	public override void _Ready()
	{
		Toggled += ToggleVsync;
		DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
	}

	private void ToggleVsync(bool toggle)
	{
		if (toggle)
		{
			_vsyncLabel.Text = "VSYNC ON";
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		}
		else
		{
			_vsyncLabel.Text = "VSYNC OFF";
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		}
	}
}
