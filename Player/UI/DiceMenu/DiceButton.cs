using Godot;
using System;

public partial class DiceButton : Button
{
	[Export] private Label _diceLabel;
	public int DiceAmount;
	
	public override void _Ready()
	{
		_diceLabel = GetParent().GetNode<Label>("Label");
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseInput)
		{
			if (mouseInput.IsActionPressed("lmb"))
			{
				UpdateDiceNumber(true);
			} else if (mouseInput.IsActionPressed("rmb"))
			{
				UpdateDiceNumber(false);
			}
		}
	}

	private void UpdateDiceNumber(bool add)
	{
		if (add)
		{
			DiceAmount++;
		}
		else
		{
			if (DiceAmount - 1 < 0) return;
			DiceAmount--;
		}

		_diceLabel.Text = DiceAmount.ToString();
	}
}
