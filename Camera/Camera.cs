using Godot;
using System;

public partial class Camera : Node3D
{
	// Movement
	[ExportGroup("Movement")]
	[Export] private float moveSpeed = 5f, moveDecline = 0.93f;
	private Vector3 motion, velocity;
	private int framesMoving = 0;

	// Zoom
	[ExportGroup("Zoom")]
	[Export] private float maxFov = 100f, minFov = 10f, defaultFov = 75f, zoomSpeed = 10f, zoomDecline = 0.93f;
	private float zoomMotion, zoomVelocity;
	private bool resetZoom = false;

	// Pan
	[ExportGroup("Pan")]
	[Export] private float panSpeed = 0.25f, panDecline = 0.9f;
	private float panMotion, panVelocity, mouseMotion, prevMouseMotion;
	
	// Top down
	[ExportGroup("Top-Down")]
	[Export] private float defaultRot = -70f, rotSpeed = 5f;
	private bool topDown = false, topDownAnim = false;
	
	private Camera3D cameraObj;
	
	public override void _Ready()
	{
		cameraObj = (Camera3D)GetNode("Camera3D");
	}

	public override void _Process(double delta)
	{
		HandlePan(delta);
		
		HandleZoom(delta);

		HandleMovement(delta);
		
		HandleTopDown(delta);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion inputEvent)
		{
			mouseMotion = inputEvent.Relative.X;
		}
	}

	private void HandleTopDown(double delta)
	{
		if (Input.IsActionJustPressed("top_down"))
		{
			topDown = !topDown;
			topDownAnim = true;
		}
		
		if (topDownAnim)
		{
			if (topDown)
			{
				// Lerp to top down
				cameraObj.Rotation = new Vector3(
					Mathf.DegToRad(Mathf.Lerp(Mathf.RadToDeg(cameraObj.Rotation.X), -90f, rotSpeed * (float)delta)), 
					0, 
					0);

				if (Mathf.IsEqualApprox(Mathf.Round(Mathf.RadToDeg(cameraObj.Rotation.X)), -90f))
				{
					topDownAnim = false;
				}
			}
			else
			{
				// Lerp to default
				cameraObj.Rotation = new Vector3(
					Mathf.DegToRad(Mathf.Lerp(Mathf.RadToDeg(cameraObj.Rotation.X), defaultRot, rotSpeed * (float)delta)), 
					0, 
					0);
				
				if (Mathf.IsEqualApprox(Mathf.Round(Mathf.RadToDeg(cameraObj.Rotation.X)), defaultRot))
				{
					topDownAnim = false;
				}
			}
		}
	}
	
	private void HandlePan(double delta)
	{
		if (Input.IsActionPressed("rmb"))
		{
			if (!Mathf.IsEqualApprox(mouseMotion, prevMouseMotion))
			{
				// Handle pan motion acceleration
				if (mouseMotion - prevMouseMotion > 0)
				{
					panMotion -= 1f;
				}
				else
				{
					panMotion += 1f;
				}

				// Handle pan direction
				if (mouseMotion > 0)
				{
					panMotion = Mathf.Abs(panMotion);
				}
				else
				{
					if (panMotion > 0)
					{
						panMotion *= -1;
					}
				}
			}
			else
			{
				panMotion = 0;
			}
		}
		else
		{
			panMotion = 0;
		}
		
		panMotion = Math.Clamp(panMotion, -2f, 2f);
		
		// // Handle velocity
		panVelocity += panMotion * panSpeed;
		panVelocity *= panDecline;
		
		Rotation += new Vector3(0, panVelocity * (float)delta, 0);

		prevMouseMotion = mouseMotion;
	}
	
	private void HandleZoom(double delta)
	{
		if (!resetZoom)
		{
			if (Input.IsActionJustPressed("scrollwheel_down"))
			{
				zoomMotion = 5f;
			} else if (Input.IsActionJustPressed("scrollwheel_up"))
			{
				zoomMotion = -5f;
			}
			else
			{
				zoomMotion = 0;
			}
		}
		else
		{
			// Handle lerping to default fov
			cameraObj.Fov = Mathf.Lerp(cameraObj.Fov, defaultFov, zoomSpeed / 1.5f * (float)delta);
			if (Mathf.IsEqualApprox(Mathf.Ceil(cameraObj.Fov), defaultFov)
			    || Mathf.IsEqualApprox(Mathf.Floor(cameraObj.Fov), defaultFov))
			{
				resetZoom = false;
				zoomVelocity = 0;
				zoomMotion = 0;
			}
		}

		// Reset zoom to default fov
		if (Input.IsActionJustPressed("mmb") && (!Mathf.IsEqualApprox(Mathf.Ceil(cameraObj.Fov), defaultFov)
		    || !Mathf.IsEqualApprox(Mathf.Floor(cameraObj.Fov), defaultFov)))
		{
			zoomMotion = 1f;
			resetZoom = true;
		}
		
		// Handle velocity
		zoomVelocity += zoomMotion * zoomSpeed;
		zoomVelocity = Math.Clamp(zoomVelocity, -100f, 100f);
		zoomVelocity *= zoomDecline;
		
		// Handle fov and fov min/max
		if (!resetZoom && (cameraObj.Fov <= maxFov && cameraObj.Fov >= minFov))
		{
			cameraObj.Fov += zoomVelocity * (float)delta;
			if (cameraObj.Fov < minFov)
			{
				cameraObj.Fov = minFov;
			} else if (cameraObj.Fov > maxFov)
			{
				cameraObj.Fov = maxFov;
			}
		}
	}
	
	private void HandleMovement(double delta)
	{
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
		velocity *= moveDecline;
		Position += velocity * (float)delta;
	}
}
