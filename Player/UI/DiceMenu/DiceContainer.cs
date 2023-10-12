using Godot;
using System;

[Tool]
public partial class DiceContainer : MarginContainer
{
	private DiceEnum _diceEnum;
	
	[Export]
	public DiceEnum DiceEnum
	{
		set
		{
			_diceEnum = value;
			UpdateLabel();
		}
		
		get => _diceEnum;
	}

	private void UpdateLabel()
	{
		GetNode<Button>("VBoxContainer/Button").Text = _diceEnum.ToString();
	}

	public int GetDiceAmount()
	{
		return GetNode<DiceButton>("VBoxContainer/Button").DiceAmount;
	}

	public void ResetDiceAmount()
	{
		GetNode<DiceButton>("VBoxContainer/Button").DiceAmount = 0;
		GetNode<Label>("VBoxContainer/Label").Text = 0.ToString();
	}
}
