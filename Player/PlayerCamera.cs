using Godot;
using System;

public partial class PlayerCamera : Node3D
{
	private Camera3D cameraObj;
	
	// Movement
	[ExportGroup("Movement")]
	[Export] private float _moveMaxSpeed = 5f, _moveAcceleration = 0.9f, _moveDeceleration = 0.93f;
	private bool _forward, _sideways;
	private Vector3 _moveDirection, _moveVelocity;

	// Panning
	[ExportGroup("Panning")]
	[Export] private float _panMaxSpeed = 7f, _panAcceleration = 0.02f, _panDeceleration = 0.85f;
	private Vector2 _mousePosition, _prevMousePosition;
	private float _panVelocity;
	private bool _captureMouseMotion;

	// Zooming
	[ExportGroup("Zooming")] 
	[Export] private float _zoomMaxFov = 85f, _zoomMinFov = 30f, _zoomDefaultFov = 75f, _zoomSpeed = 30f, _zoomDeceleration = 0.85f;
	private float _zoomVelocity;
	private bool _zoomEnableDown = true, _zoomEnableUp = true, _zoomReset;
	
	// Tactical View
	[ExportGroup("Tactical View")] 
	[Export] private float _tactSpeed = 5f;
	private bool _tacticalView;
	private float _tactDefaultRot = -50f;

	private PlayerVariables _playerVariables;
	
	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Convert.ToInt32(GetParent().Name));
		((Camera3D)GetNode("Camera3D")).Current = IsMultiplayerAuthority();
	}
	
	public override void _Ready()
	{
		_playerVariables = GetNode<PlayerVariables>("/root/PlayerVariables");
		
		cameraObj = (Camera3D)GetNode("Camera3D");
		Position = new Vector3(0, 3, 0);
	}

	public override void _Input(InputEvent @event)
	{
		if (_captureMouseMotion && @event is InputEventMouseMotion inputEvent)
		{
			_mousePosition = inputEvent.Position;
			if (_prevMousePosition.Equals(Vector2.Zero))
			{
				_prevMousePosition = _mousePosition;
			}
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		
		HandleMovement(delta);
		HandlePan(delta);
		HandleZoom(delta);
		HandleTopDown(delta);
	}

	private void HandleMovement(double delta)
	{
		_forward = true;
		_sideways = true;

		#region Forward_Movement
		if (Input.IsActionPressed("move_forward") && Input.IsActionPressed("move_backward") ||
		    !Input.IsActionPressed("move_forward") && !Input.IsActionPressed("move_backward"))
		{
			_forward = false;
		}

		if (_forward && _playerVariables.GetCameraEnabled())
		{
			if (Input.IsActionPressed("move_forward"))
			{
				_moveVelocity.X -= Transform.Basis.Z.X * _moveAcceleration;
				_moveVelocity.Z -= Transform.Basis.Z.Z * _moveAcceleration;
			} else if (Input.IsActionPressed("move_backward"))
			{
				_moveVelocity.X += Transform.Basis.Z.X * _moveAcceleration;
				_moveVelocity.Z += Transform.Basis.Z.Z * _moveAcceleration;
			}
		}
		#endregion

		#region Sideways_Movement
		if (Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right") ||
		    !Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right"))
		{
			_sideways = false;
		}

		if (_sideways && _playerVariables.GetCameraEnabled())
		{
			if (Input.IsActionPressed("move_left"))
			{
				_moveVelocity.X -= Transform.Basis.X.X * _moveAcceleration;
				_moveVelocity.Z -= Transform.Basis.X.Z * _moveAcceleration;
			} else if (Input.IsActionPressed("move_right"))
			{
				_moveVelocity.X += Transform.Basis.X.X * _moveAcceleration;
				_moveVelocity.Z += Transform.Basis.X.Z * _moveAcceleration;
			}
		}
		#endregion
		
		// Check small velocities
		if (_moveVelocity.Equals(Vector3.Zero)) return;
		if (_moveVelocity.X is < 0.01f and > -0.01f) _moveVelocity.X = 0;
		if (_moveVelocity.Z is < 0.01f and > -0.01f) _moveVelocity.Z = 0;
		
		// Clamp vector length
		_moveVelocity.LimitLength(_moveMaxSpeed);
		
		// Update position and decelerate velocity
		Position += _moveVelocity * (float)delta;
		_moveVelocity *= _moveDeceleration;
	}

	private void HandlePan(double delta)
	{
		if (Input.IsActionPressed("rmb") && _playerVariables.GetCameraEnabled())
		{
			if (Input.IsActionJustPressed("rmb"))
			{
				_mousePosition = Vector2.Zero;
				_prevMousePosition = Vector2.Zero;
			}
			_captureMouseMotion = true;
			_panVelocity += (_prevMousePosition.X - _mousePosition.X) * _panAcceleration;
		}
		else
		{
			_captureMouseMotion = false;
		}

		// Check small velocities
		if (_panVelocity == 0) return;
		if (_panVelocity is < 0.01f and > -0.01f) _panVelocity = 0;
		
		// Clamp pan speed
		_panVelocity = Mathf.Clamp(_panVelocity, _panMaxSpeed * -1, _panMaxSpeed);
		
		// Update rotation, mouse position, and decelerate panning
		Rotation += new Vector3(0, _panVelocity * (float)delta, 0);
		_prevMousePosition = _mousePosition;
		_panVelocity *= _panDeceleration;
	}
	
	private void HandleZoom(double delta)
	{
		if (_zoomReset)
		{
			cameraObj.Fov = Mathf.Lerp(cameraObj.Fov, _zoomDefaultFov, _zoomSpeed / 3f * (float)delta);
			if (Mathf.IsEqualApprox(Mathf.Ceil(cameraObj.Fov), _zoomDefaultFov)
			    || Mathf.IsEqualApprox(Mathf.Floor(cameraObj.Fov), _zoomDefaultFov)
			    || Input.IsActionJustPressed("scrollwheel_down")
			    || Input.IsActionJustPressed("scrollwheel_up"))
			{
				_zoomReset = false;
			}
		}

		if (_playerVariables.GetCameraEnabled())
		{
			if (Input.IsActionJustPressed("scrollwheel_down") && _zoomEnableDown)
			{
				_zoomVelocity += _zoomSpeed;
			} else if (Input.IsActionJustPressed("scrollwheel_up") && _zoomEnableUp)
			{
				_zoomVelocity -= _zoomSpeed;
			} else if (Input.IsActionJustPressed("mmb"))
			{
				_zoomReset = true;
			}
		}
		
		// Check small velocities
		if (_zoomVelocity == 0) return;
		if (_zoomVelocity is < 0.01f and > -0.01f) _zoomVelocity = 0;

		// Handle min/max fovs
		if (_zoomEnableUp && cameraObj.Fov < _zoomMinFov)
		{
			_zoomEnableUp = false;
			_zoomVelocity = -10;
		} else if (_zoomEnableDown && cameraObj.Fov > _zoomMaxFov)
		{
			_zoomEnableDown = false;
			_zoomVelocity = 10;
		}
		
		// Update fov and decelerate zoom
		cameraObj.Fov += _zoomVelocity * (float)delta;
		_zoomVelocity *= _zoomDeceleration;
		
		// Handle allowing zoom at min/max fovs
		if (!_zoomEnableUp && cameraObj.Fov > _zoomMinFov) _zoomEnableUp = true;
		if (!_zoomEnableDown && cameraObj.Fov < _zoomMaxFov) _zoomEnableDown = true;
	}

	private void HandleTopDown(double delta)
	{
		if (Input.IsActionJustPressed("tactical_view") && _playerVariables.GetCameraEnabled())
		{
			_tacticalView = !_tacticalView;
		}

		if (_tacticalView)
		{
			if (Mathf.IsEqualApprox(Mathf.RadToDeg(cameraObj.Rotation.X), -90f)) return;
			
			// Lerp to tactical
			cameraObj.Rotation = new Vector3(
				Mathf.DegToRad(Mathf.Lerp(Mathf.RadToDeg(cameraObj.Rotation.X), -90f, _tactSpeed * (float)delta)), 
				0, 
				0);
		}
		else
		{
			if (Mathf.IsEqualApprox(Mathf.RadToDeg(cameraObj.Rotation.X), _tactDefaultRot)) return;
			
			// Lerp to default
			cameraObj.Rotation = new Vector3(
				Mathf.DegToRad(Mathf.Lerp(Mathf.RadToDeg(cameraObj.Rotation.X), _tactDefaultRot, _tactSpeed * (float)delta)), 
				0, 
				0);
		}
	}
}
