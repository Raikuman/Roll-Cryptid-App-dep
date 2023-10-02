using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using Godot.Collections;

public partial class MouseManager : Node3D
{

	private MeshInstance3D _mouseMesh;
	private Camera3D _camera;
	private World3D _world;
	private PhysicsDirectSpaceState3D _spaceState;
	private Vector3 _posPrev, _posCurr;
	private Viewport _viewport;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Convert.ToInt32(GetParent().Name));
		_camera = (Camera3D)GetParent().GetNode("CameraHolder/Camera3D");
		_mouseMesh = (MeshInstance3D)GetNode("MouseMesh");
		_viewport = GetViewport();
		_world = GetWorld3D();
		_spaceState = _world.DirectSpaceState;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleMouse();
	}

	private void HandleMouse()
	{
		var raycast = Raycast();

		// Hide mouse when off object
		if (raycast.Count > 0)
		{
			if (!_mouseMesh.Visible) _mouseMesh.Show();
		}
		else
		{
			if (_mouseMesh.Visible) _mouseMesh.Hide();
		}
		
		// Return on no cast
		if (raycast.Count == 0) return;
		
		// Return on same position
		if (((Vector3)raycast["position"]).Equals(_posPrev)) return;
		
		// Update mouse position
		_mouseMesh.Position = (Vector3)raycast["position"];
		_posPrev = _mouseMesh.Position;
	}

	private Dictionary Raycast()
	{
		var rayStart = _camera.ProjectRayOrigin(_viewport.GetMousePosition());
		var rayEnd = rayStart + _camera.ProjectRayNormal(_viewport.GetMousePosition()) * 500;
			
		var query = PhysicsRayQueryParameters3D.Create(
			rayStart,
			rayEnd, 1);
		
		return _spaceState.IntersectRay(query);
	}
}
