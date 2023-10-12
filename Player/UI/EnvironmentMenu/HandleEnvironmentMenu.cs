using Godot;
using System;

public partial class HandleEnvironmentMenu : Control
{
	private HSlider _timeSlider;
	private Label _timeLabel;
	private SpinBox _cycleSpinBox;
	private CheckBox _cycleCheckBox;
	private HBoxContainer _menuContainer;
	private Button _menuButton;
	private SkyControl _skyControl;
	private bool _sliderUpdate;
	private TimeSpan _time;
	private AnimateToLocation _animator;
	
	public override void _Ready()
	{
		_timeSlider = GetNode<HSlider>("HBoxContainer/ControlInfo/PanelContainer/Control/ControlTIme/HSlider");
		_timeLabel = GetNode<Label>("HBoxContainer/ControlInfo/PanelContainer/Control/ControlTIme/LabelTime");
		_cycleSpinBox = GetNode<SpinBox>("HBoxContainer/ControlInfo/PanelContainer/Control/ControlCycleSpeed/SpinBox");
		_cycleCheckBox = GetNode<CheckBox>("HBoxContainer/ControlInfo/PanelContainer/Control/ControlCycle/CheckBox");
		_menuContainer = GetNode<HBoxContainer>("HBoxContainer");
		_menuButton = GetNode<Button>("HBoxContainer/ControlButton/Button");
		_skyControl = GetParent().GetNode<SkyControl>("Environment");
		
		_cycleSpinBox.Value = _skyControl._cycleSpeed;
		_cycleSpinBox.GetLineEdit().FocusEntered += () =>
		{
			_cycleSpinBox.GetLineEdit().ReleaseFocus();;
		};

		_animator = GetNode<AnimateToLocation>("AnimateToLocation");
		
		if (!GetNode<MultiplayerVariables>("/root/MultiplayerVariables").GetHost())
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		_timeSlider.Value = _skyControl.TimeOfDay;

		UpdateTime();
	}

	private void UpdateTime()
	{
		_time = TimeSpan.FromMinutes(_skyControl.TimeOfDay + 360f);
		if (_timeLabel.Text.Equals(_time.ToString(@"hh\:mm"))) return;
		
		if (_time.Days > 0)
		{
			_time = _time.Subtract(TimeSpan.FromDays(1));
		}

		_timeLabel.Text = _time.ToString(@"hh\:mm");
	}

	// Handle menu button
	private void _menu_pressed()
	{
		_animator?.Animate();

		if (_animator == null) return;
		if (_animator.IsOnTarget())
		{
			_menuButton.Text = ">";
		}
		else
		{
			_menuButton.Text = "<";
		}
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
		_skyControl.TimeOfDay = (float)_timeSlider.Value;

		if (_cycleCheckBox.ButtonPressed)
		{
			_skyControl._enableCycle = false;
		}
	}

	private void _slider_on_drag_ended(bool valueChanged)
	{
		_sliderUpdate = false;
		
		if (_cycleCheckBox.ButtonPressed)
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
