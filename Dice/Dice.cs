using Godot;
using System;

public partial class Dice : RigidBody3D
{
	[Export(PropertyHint.Range, "1, 20")] private int _maxVel = 7;
	public Vector3 ThrowPosition, ThrowAngle;
	public int UpmostFace, ThrowerId;
	public bool CompletedRoll;
	public string DiceEnum;
	private Timer _timer;
	
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		Freeze = true;
		
		// Only calculate physics on server
		if (!IsMultiplayerAuthority())
		{
			FreezeMode = FreezeModeEnum.Static;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Freeze) return;

		CheckForFreeze();
		
		// Reset position if falling out of world
		if (Position.Y < -20)
		{
			Position = ThrowPosition;
		}
	}

	public void ThrowDice()
	{
		Freeze = false;

		var rng = new RandomNumberGenerator();
		
		var generateAngle = rng.RandfRange(Mathf.DegToRad(40f), Mathf.DegToRad(360f));
		var randomAngle = new Vector3(
			generateAngle * rng.RandiRange(-3, 3), 
			generateAngle * rng.RandiRange(-3, 3), 
			generateAngle * rng.RandiRange(-3, 3));
		AngularVelocity = randomAngle;

		ThrowAngle.Y += Mathf.DegToRad(40f);
		LinearVelocity = ThrowAngle * _maxVel;
		Position = ThrowPosition;
	}

	private void _timer_timeout()
	{
		var upness = GetSideAndUpness();
		if (upness.Item1 < 0.25f)
		{
			_timer.Start();
			ThrowDice();
		}
		else
		{
			UpmostFace = upness.Item2;
			Freeze = true;
			CompletedRoll = true;
		}
	}
	
	private void CheckForFreeze()
	{
		if (LinearVelocity.Length() > 0.001 || AngularVelocity.Length() > 0.001 && !CompletedRoll)
		{
			_timer.Start();
		}
	}

	private (float, int) GetSideAndUpness()
	{
		var maxUpness = 0.0f;
		var face = 0;
		var normalsNode = GetNode("Normals");

		foreach (var normal in normalsNode.GetChildren())
		{
			var script = (DiceNormal)normal;
			var normalY = normal.GetNode<MeshInstance3D>("MeshInstance3D").GlobalTransform.Basis.Y;
			var upness = normalY.Dot(Vector3.Up);
			
			if (upness > maxUpness)
			{
				maxUpness = upness;
				face = script.face;
			}
		}

		return (maxUpness, face);
	}
}
