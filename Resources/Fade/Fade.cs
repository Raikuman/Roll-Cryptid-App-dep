using Godot;
using System;

public partial class Fade : Control
{
	[Export] private ColorRect colorRect;

	private bool animate, enableFunction;
	private float original, target;
	private double elapsedTime, targetTime = 5;
	
	public override void _Ready()
	{
		Hide();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!animate) return;

		elapsedTime += delta;
		
		var modulate = colorRect.Modulate;
		modulate.A = Mathf.Lerp(original, target, Mathf.SmoothStep(0, 1, (float)(elapsedTime / targetTime)));
		colorRect.Modulate = modulate;

		if (elapsedTime > targetTime)
		{
			elapsedTime = 0;
			animate = false;
			enableFunction = true;

			// Ensure fadeout hides fade UI
			if (colorRect.Modulate.A.Equals(0f))
			{
				Hide();
			}
		}
	}

	public bool AllowFunction()
	{
		return enableFunction;
	}
	
	public void FadeIn(double targetTime = 0.6)
	{
		// Ensure modulation starts at 0 alpha
		var modulate = colorRect.Modulate;
		modulate.A = 0;
		colorRect.Modulate = modulate;
		
		Show();
		enableFunction = false;
		animate = true;
		this.targetTime = targetTime;
		original = 0f;
		target = 1f;
	}

	public void FadeOut(double targetTime = 0.6)
	{
		Show();
		enableFunction = false;
		animate = true;
		this.targetTime = targetTime;
		original = 1f;
		target = 0f;
	}
}
