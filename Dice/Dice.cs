using Godot;
using System;

public partial class Dice : RigidBody3D
{
	[Export] private int _numSides = 20, _maxVel = 10;
	public Vector3 _initialPosition;
	public int _upmostFace, _thrower;
	private double _freezeTimer, _freezeThreshold = 1;
	
	public override void _Ready()
	{
		Position = _initialPosition;
		Freeze = true;

		if (!IsMultiplayerAuthority())
		{
			FreezeMode = FreezeModeEnum.Static;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Position.Y < -20)
		{
			Position = _initialPosition;
		}
	}

	public override void _Process(double delta)
	{
		if (Freeze) return;
		
		if (LinearVelocity.Length() < 0.1 && AngularVelocity.Length() < 0.1 && Freeze == false)
		{
			_freezeTimer += delta;
			if (_freezeTimer > _freezeThreshold)
			{
				Freeze = true;
				// GetSide
			}
		}
		else
		{
			_freezeTimer = 0;
		}
	}
	
	public void ThrowDice(Vector3 throwPosition, Vector3 throwAngle)
	{
		Freeze = false;

		var rng = new RandomNumberGenerator();

		var generateAngle = rng.RandfRange(Mathf.DegToRad(40f), Mathf.DegToRad(360f));
		var randomAngle = new Vector3(
			generateAngle * rng.RandiRange(-7, 7), 
			generateAngle * rng.RandiRange(-7, 7), 
			generateAngle * rng.RandiRange(-7, 7));
		AngularVelocity = randomAngle;

		throwAngle.Y += Mathf.DegToRad(40f);
		LinearVelocity = throwAngle * _maxVel;
		Position = throwPosition;
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
				_upmostFace = script.face;
			}
		}
	}
}
