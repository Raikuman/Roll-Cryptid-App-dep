using Godot;
using System;

public partial class Camera : Node3D
{
	private Vector3 motion, initialRotation, velocity;
	private float moveSpeed = 10f;
	private int framesMoving = 0;
	
	private Camera3D cameraObj;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cameraObj = (Camera3D)GetNode("Camera3D");
		initialRotation = Transform.Basis.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("rmb"))
		{
			Rotation += new Vector3(0, 15f, 0);
		}

		bool forward = true, sideways = true;
		
		#region Forward_Movement
		if (!Input.IsActionPressed("move_backward") && !Input.IsActionPressed("move_forward") 
		    || Input.IsActionPressed("move_backward") && Input.IsActionPressed("move_forward"))
		{
			forward = false;
		}
		
		if (Input.IsActionPressed("move_backward"))
		{
			motion.X += Transform.Basis.Z.X;
			motion.Z += Transform.Basis.Z.Z;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			motion.X -= Transform.Basis.Z.X;
			motion.Z -= Transform.Basis.Z.Z;
		}
		#endregion

		#region Sideways_Movement
		if (!Input.IsActionPressed("move_right") && !Input.IsActionPressed("move_left") 
		    || Input.IsActionPressed("move_right") && Input.IsActionPressed("move_left"))
		{
			sideways = false;
		}

		if (Input.IsActionPressed("move_right"))
		{
			motion.X += Transform.Basis.X.X;
			motion.Z += Transform.Basis.X.Z;
		}
		else if (Input.IsActionPressed("move_left"))
		{
			motion.X -= Transform.Basis.X.X;
			motion.Z -= Transform.Basis.X.Z;
		}
		#endregion

		// No movement
		if (!forward && !sideways)
		{
			GD.Print("Hit");
			motion = Vector3.Zero;
		}
		
		motion = motion.Normalized();

		// Track number of frames motion can cause movement
		if (motion != Vector3.Zero)
		{
			framesMoving++;
		}
		else
		{
			framesMoving = 0;
		}

		// Remove single-frame movement to prevent jitter
		if (framesMoving > 1)
		{
			velocity += motion * moveSpeed;
		}
		
		// Handle velocity and position
		velocity *= 0.93f;
		Position += velocity * (float)delta;
	}
}
