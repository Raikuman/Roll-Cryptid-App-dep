using Godot;
using System;

public partial class Dice : RigidBody3D
{
	[Export] private int maxAngle = 20, maxVel = 4;
	private Vector3 initialPosition;
	public int upmostFace = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		initialPosition = GlobalTransform.Origin;
		throwDice();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("lmb"))
		{
			throwDice();
		}

		if (LinearVelocity.Length() < 0.1 && AngularVelocity.Length() < 0.1 && Freeze == false)
		{
			Freeze = true;
		}
		
		getSide();
	}
	
	private void throwDice()
	{
		GlobalPosition = initialPosition;
		Freeze = false;

		var rng = new RandomNumberGenerator();

		var randomAngle = new Vector3(
			rng.RandiRange(-maxAngle, maxAngle), 
			rng.RandiRange(-maxAngle, maxAngle), 
			rng.RandiRange(-maxAngle, maxAngle));
		AngularVelocity = randomAngle;

		var randomVelocity = new Vector3(
			rng.RandiRange(-maxVel, maxVel),
			rng.RandiRange(-maxVel, maxVel),
			rng.RandiRange(-maxVel, maxVel));
		LinearVelocity = randomVelocity;
	}

	private void getSide()
	{
		var maxUpness = 0.0;
		var normalsNode = GetNode("Normals");
		foreach (var normalNode in normalsNode.GetChildren())
		{
			var script = (DiceNormal)normalNode;

			var normalY = normalNode.GetNode<MeshInstance3D>("MeshInstance3D").GlobalTransform.Basis.Y;
			var upness = normalY.Dot(Vector3.Up);

			if (upness > maxUpness)
			{
				maxUpness = upness;
				upmostFace = script.face;
			}
		}
	}
}
