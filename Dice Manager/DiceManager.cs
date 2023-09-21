using Godot;
using System;

public partial class DiceManager : Node3D
{
	[Export] private float timeShown = 5f;
	private float currentTime;
	private bool startTimer;
	
	private PackedScene d20 = ResourceLoader.Load<PackedScene>("res://Dice/dice.tscn");
	private int numDice, prevNumDice;
	private Label label;
	private Node3D[] diceArray = Array.Empty<Node3D>();
	private int[] diceRolls;
	private Node diceHolder;

	private PackedScene logEntry = ResourceLoader.Load<PackedScene>("res://margin_container.tscn");
	private Node rollLog;
	
	public override void _Ready()
	{
		label = (Label)GetNode("CanvasLayer/MarginContainer/VBoxContainer/NumDice");
		diceHolder = new Node3D();
		diceHolder.Name = "DiceHolder";
		AddChild(diceHolder);

		rollLog = GetNode("ScrollContainer/VBoxContainer");
	}

	public override void _PhysicsProcess(double delta)
	{
		CheckCompleteRoll();

		if (startTimer)
		{
			currentTime += (float)delta;
		}

		if (currentTime > timeShown)
		{
			RemoveDice();
		}
	}

	private void RemoveDice()
	{
		if (diceHolder.GetChildren().Count == 0) return;
		
		startTimer = false;
		currentTime = 0;
		
		diceHolder.QueueFree();
		diceHolder = new Node3D();
		diceHolder.Name = "DiceHolder";
		diceArray = Array.Empty<Node3D>();
		diceRolls = Array.Empty<int>();
		AddChild(diceHolder);
	}

	private void CheckCompleteRoll()
	{
		if (diceArray.Length == 0 || startTimer) return;
		
		// Get dice that are finished rolling
		var numComplete = 0;
		diceRolls = new int[diceArray.Length];
		for (var i = 0; i < diceArray.Length; i++)
		{
			if (diceArray[i] is not Dice dice) continue;
			
			diceRolls[i] = dice.upmostFace;

			if (dice.Freeze)
			{
				numComplete++;
			}
		}
		
		// Complete roll when all dice are done rolling
		if (numComplete != diceArray.Length) return;

		var rollString = Time.GetTimeStringFromSystem() + " | Rolled " + prevNumDice + "d20: ";
		var rolledNumbers = "";
		var total = 0;
		
		for (var i = 0; i < diceRolls.Length; i++)
		{
			total += diceRolls[i];
			rolledNumbers += diceRolls[i];

			if (i < diceRolls.Length - 1)
			{
				rolledNumbers += ", ";
			}
		}

		rollString += total + " (" + rolledNumbers + ")";

		var newLog = logEntry.Instantiate();
		var logLabel = (Label)newLog.GetNode("Label");
		logLabel.Text = rollString;
		rollLog.AddChild(newLog);
		
		startTimer = true;
	}
	
	private void _on_roll_pressed()
	{
		if (numDice == 0) return;
		RemoveDice();
		
		diceArray = new Node3D[numDice];

		for (var i = 0; i < diceArray.Length; i++)
		{
			var newDice = d20.Instantiate();
			diceHolder.AddChild(newDice);
			
			if (newDice is Dice dice)
			{
				dice.throwDice();
			}
			
			diceArray[i] = (Node3D)newDice;
		}
		
		label.Text = "Num dice: 0";
		prevNumDice = numDice;
		numDice = 0;
	}

	private void _increase_dice()
	{
		numDice++;
		label.Text = "Num dice: " + numDice;
	}

	private void _decrease_dice()
	{
		if (numDice <= 0) return;
		
		numDice--;
		label.Text = "Num dice: " + numDice;
	}
}
