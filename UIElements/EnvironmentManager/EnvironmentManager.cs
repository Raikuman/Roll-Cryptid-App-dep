using Godot;
using System;

public partial class EnvironmentManager : Node
{
	private HSlider _slider;
	private SpinBox _spinBox;
	private CheckBox _checkBox;
	private SkyControl _skyControl;
	private bool _sliderUpdate;
	
	public override void _Ready()
	{
		_slider = GetNode<HSlider>("Control/HSlider");
		_spinBox = GetNode<SpinBox>("Control/SpinBox");
		_checkBox = GetNode<CheckBox>("Control/CheckBox");
		_skyControl = GetNode<SkyControl>("Environment");
		
		_spinBox.Value = _skyControl._cycleSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		_slider.Value = _skyControl.TimeOfDay;
	}

	// Handle checkbox
	private void _checkbox_toggled(bool buttonPressed)
	{
		_skyControl._enableCycle = buttonPressed;
	}
	
	// Handle spinbox
	private void _spinbox_on_value_changed(float value)
	{
		_skyControl._cycleSpeed = value;
	}
	
	// Handle slider
	private void _slider_on_drag_started()
	{
		_sliderUpdate = true;
		_skyControl.TimeOfDay = (float)_slider.Value;

		if (_checkBox.ButtonPressed)
		{
			_skyControl._enableCycle = false;
		}
	}

	private void _slider_on_drag_ended(bool valueChanged)
	{
		_sliderUpdate = false;
		
		if (_checkBox.ButtonPressed)
		{
			_skyControl._enableCycle = true;
		}
	}
	
	private void _slider_on_value_changed(float value)
	{
		if (_sliderUpdate)
		{
			_skyControl.TimeOfDay = value;
		}
	}
}
