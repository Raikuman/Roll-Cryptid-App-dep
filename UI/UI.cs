using Godot;
using System;

public partial class UI : CanvasLayer
{
	private Node dice1, dice2;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// dice1 = GetNode("../Dice/Dice1");
		// dice2 = GetNode("../Dice/Dice2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// ((Label)GetNode("Results/MarginContainer/Label")).Text = ((Dice)dice1).upmostFace.ToString();
		// ((Label)GetNode("Results/MarginContainer2/Label")).Text = ((Dice)dice2).upmostFace.ToString();
	}
}
