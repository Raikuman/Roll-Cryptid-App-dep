using Godot;
using System;

public partial class new_script : CharacterBody3D
{
	private const float speed = 5f;
	private const float acceleration = 0.5f;
	private double gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
	
	
	
	private Vector3 velocity = Vector3.Zero;
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		var inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		var direction = (GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, acceleration);
		}

		if (!IsOnFloor())
		{
			velocity.Y -= (float)gravity * (float)delta;
		}

		Velocity = velocity;
		MoveAndSlide();
		
		GD.Print(IsOnFloor());
	}
}
