using Godot;
using System;

public partial class SkyControl : WorldEnvironment
{
	private float _timeOfDay = 150;
	
	[Export(PropertyHint.Range, "0, 1200")]
	public float TimeOfDay
	{
		set
		{
			_timeOfDay = value;
			UpdateSky();
		}
		
		get => _timeOfDay;
	}

	[Export] private DirectionalLight3D _sunLight, _moonLight;
	[Export] public bool _enableCycle;
	[Export] public float _cycleSpeed = 20;

	private Node3D _cycle;

	private Gradient _sunColor, _sunTop, _sunHorizon, _moonColor, _moonTop, _moonHorizon;
	
	public override void _Ready()
	{
		_cycle = GetNode<Node3D>("Cycle");
		
		_sunLight = GetNode<DirectionalLight3D>("Cycle/Sun");
		_sunColor = ResourceLoader.Load<Gradient>("res://Sky/Gradients/SunGradient.tres");
		_sunTop = ResourceLoader.Load<Gradient>("res://Sky/Gradients/SunTop.tres");
		_sunHorizon = ResourceLoader.Load<Gradient>("res://Sky/Gradients/SunHorizon.tres");
		
		_moonLight = GetNode<DirectionalLight3D>("Cycle/Moon");
		_moonColor = ResourceLoader.Load<Gradient>("res://Sky/Gradients/MoonGradient.tres");
		_moonTop = ResourceLoader.Load<Gradient>("res://Sky/Gradients/MoonTop.tres");
		_moonHorizon = ResourceLoader.Load<Gradient>("res://Sky/Gradients/MoonHorizon.tres");
		
		UpdateSky();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_enableCycle || _cycleSpeed == 0) return;

		_timeOfDay += (float)delta * _cycleSpeed;
		if (_timeOfDay > 1200) _timeOfDay = 0;

		UpdateSky();
	}

	private void UpdateSky()
	{
		UpdateRotation();
		UpdateLights();
	}

	private void UpdateRotation()
	{
		if (!IsMultiplayerAuthority()) return;
		
		_cycle.Rotation = new Vector3(Mathf.DegToRad(_timeOfDay * -0.3f), 0, 0);
	}

	private void UpdateLights()
	{
		var light = Mathf.Sin(Mathf.Pi/600 * _timeOfDay);
		if (light > 0)
		{
			// Update light
			_sunLight.LightEnergy = Mathf.Clamp(light * 2, 0.1f, 2f);
			_sunLight.LightColor = _sunColor.Sample(light);
			
			// Update environment
			Environment.Sky.SkyMaterial.Set("sky_top_color", _sunTop.Sample(light));
			Environment.Sky.SkyMaterial.Set("sky_horizon_color", _sunHorizon.Sample(light));
		}
		else
		{
			// Update light
			light *= -1;
			_moonLight.LightEnergy = Mathf.Clamp(light * 2, 0.5f, 1.3f);
			_moonLight.LightColor = _moonColor.Sample(light);
			
			// Update environment
			Environment.BackgroundEnergyMultiplier = Mathf.Clamp(1 - light, 0.4f, 0.8f);
			Environment.Sky.SkyMaterial.Set("sky_top_color", _moonTop.Sample(light));
			Environment.Sky.SkyMaterial.Set("sky_horizon_color", _moonHorizon.Sample(light));
		}
	}
}
