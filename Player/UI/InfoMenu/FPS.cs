using Godot;
using System;

public partial class FPS : Label
{
	public override void _PhysicsProcess(double delta)
	{
		Text = Engine.GetFramesPerSecond().ToString();
	}
}
