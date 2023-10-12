using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DiceManager : Node
{
	[Export] private Label _diceLabel;
	[Export] private int _diceRows = 4;
	private int _numDice;

	private void _on_roll_pressed()
	{
		if (_numDice == 0) return;
		AddDice();
	}

	private void AddDice()
	{
		// Retrieve camera vectors
		var cameraHolder = (Node3D)GetViewport().GetCamera3D().GetParent();
		var camera = GetViewport().GetCamera3D();
	
		var up = camera.GlobalTransform.Basis.Y;
		var left = camera.GlobalTransform.Basis.X;

		var throwPosition = cameraHolder.Position + up * -2;
		var throwLeft = left * 2;
		
		Rpc("_add_dice", Multiplayer.GetUniqueId(), _numDice, throwPosition, camera.GlobalTransform);
		
		_numDice = 0;
		_diceLabel.Text = "Num dice: " + _numDice;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _add_dice(int throwId, int numDice, Vector3 initialPosition, Transform3D cameraTransform)
	{
		if (!IsMultiplayerAuthority()) return;
		
		var diceResource = ResourceLoader.Load<PackedScene>("res://Dice/D20.tscn").Instantiate();
	
		var left = cameraTransform.Basis.X * 2;
		var backward = cameraTransform.Basis.Z * -2;
		var down = cameraTransform.Basis.Y * -2;

		var diceInRow = 0;
		var rows = 0;
		var set = 0;
		for (var i = 0; i < numDice; i++)
		{
			var newDice = diceResource.Duplicate();
			((Dice)newDice).ThrowerId = throwId;
			((Dice)newDice).ThrowPosition = initialPosition + left;
			newDice.Name = "D20-";
			GetParent().GetNode("Dice").AddChild(newDice, true);

			// Setup position
			var spawnPos = initialPosition;
			var rowPos = left;

			// Create new set
			spawnPos += down * set;
			
			// Create new rows
			spawnPos += backward * rows;
			
			// Alternate row position
			if (i % 2 != 0) rowPos *= -1;

			// Set row position
			spawnPos += rowPos * diceInRow * 0.5f;

			// Throw dice
			if (newDice is Dice dice)
			{
				dice.ThrowDice();
			}

			// Update rows and sets
			if (i > 0 && i % _diceRows == 0)
			{
				diceInRow = 0;
				rows++;
			}

			if (rows > _diceRows)
			{
				rows = 0;
				set++;
			}

			diceInRow++;
		}
	}
	
	private void _increase_dice()
	{
		_numDice++;
		_diceLabel.Text = "Num dice: " + _numDice;
	}
	
	private void _decrease_dice()
	{
		if (_numDice <= 0) return;
		
		_numDice--;
		_diceLabel.Text = "Num dice: " + _numDice;
	}

	// [Export] private Label diceLabel;
	// [Export] private Node rollLog, diceNode;
	// [Export] private float timeShown = 5f;
	// private float currentTime;
	// private bool startTimer, rolling;
	//
	// private int numDice, prevNumDice;
	// private List<Node3D> diceArray = new ();
	// private int[] diceRolls;
	//
	// public override void _PhysicsProcess(double delta) 
	// {
	// 	CheckCompleteRoll();
	//
	// 	if (startTimer)
	// 	{
	// 		currentTime += (float)delta;
	// 		if (currentTime > timeShown)
	// 		{
	// 			DeleteDice();
	// 		}
	// 	}
	// }
	//
	// private void RemoveDice()
	// {
	// 	if (diceNode.GetChildren().Count == 0) return;
	// 	
	// 	startTimer = false;
	// 	currentTime = 0;
	//
	// 	foreach (var child in diceNode.GetChildren())
	// 	{
	// 		diceNode.RemoveChild(child);
	// 		child.QueueFree();
	// 	}
	// 	
	// 	diceRolls = Array.Empty<int>();
	// }
	//
	// private void CheckCompleteRoll()
	// {
	// 	if (!rolling) return;
	// 	
	// 	// Get dice that are finished rolling
	// 	var numComplete = 0;
	// 	diceRolls = new int[diceArray.Count];
	// 	for (var i = 0; i < diceArray.Count; i++)
	// 	{
	// 		if (diceArray[i] is not Dice dice) continue;
	// 		
	// 		diceRolls[i] = dice.upmostFace;
	//
	// 		if (dice.Freeze)
	// 		{
	// 			numComplete++;
	// 		}
	// 	}
	// 	
	// 	// Complete roll when all dice are done rolling
	// 	if (numComplete != diceArray.Count) return;
	//
	// 	var rollString = Time.GetTimeStringFromSystem() + " | Rolled " + prevNumDice + "d20: ";
	// 	var rolledNumbers = "";
	// 	var total = 0;
	// 	
	// 	for (var i = 0; i < diceRolls.Length; i++)
	// 	{
	// 		total += diceRolls[i];
	// 		rolledNumbers += diceRolls[i];
	//
	// 		if (i < diceRolls.Length - 1)
	// 		{
	// 			rolledNumbers += ", ";
	// 		}
	// 	}
	//
	// 	rollString += total + " (" + rolledNumbers + ")";
	// 	
	// 	var newLog = ResourceLoader.Load<PackedScene>("res://UIElements/DiceManager/Log.tscn").Instantiate();
	// 	((Label)newLog).Text = rollString;
	// 	rollLog.AddChild(newLog);
	// 	
	// 	startTimer = true;
	// 	rolling = false;
	// }
	//
	// private void _on_roll_pressed()
	// {
	// 	if (numDice == 0) return;
	// 	DeleteDice();
	// 	AddDice();
	// 	//diceArray = new Node3D[numDice];
	// 	
	// 	// for (var i = 0; i < numDice; i++)
	// 	// {
	// 	// 	var newDice = ResourceLoader.Load<PackedScene>("res://Dice/D20.tscn").Instantiate();
	// 	// 	((Dice)newDice).thrower = Multiplayer.GetUniqueId();
	// 	// 	diceNode.AddChild(newDice, true);
	// 	//
	// 	// 	// Retrieve camera forward
	// 	// 	var cameraHolder = (Node3D)GetViewport().GetCamera3D().GetParent();
	// 	// 	var camera = GetViewport().GetCamera3D();
	// 	//
	// 	// 	var up = camera.GlobalTransform.Basis.Y;
	// 	// 	var left = camera.GlobalTransform.Basis.X;
	// 	// 	
	// 	// 	// Calculate camera position
	// 	// 	if (i % 2 != 0)
	// 	// 	{
	// 	// 		left *= -i;
	// 	// 	}
	// 	// 	
	// 	// 	var cameraPos = cameraHolder.Position + up * -2 + left * 2;
	// 	// 	
	// 	// 	if (newDice is Dice dice)
	// 	// 	{
	// 	// 		dice.throwDice(cameraPos, camera.GlobalTransform.Basis.Z.Normalized() * -1);
	// 	// 	}
	// 	// 	
	// 	// 	diceArray.Add((Node3D)newDice);
	// 	// }
	// 	
	// 	diceLabel.Text = "Num dice: 0";
	// 	prevNumDice = numDice;
	// 	numDice = 0;
	// 	rolling = true;
	// }
	//
	// private void AddDice
	// {
	// 	Rpc("_add_dice", numDice);
	// }
	//
	// [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	// private void _add_dice(int diceAmount)
	// {
	// 	for (var i = 0; i < diceAmount; i++)
	// 	{
	// 		var newDice = ResourceLoader.Load<PackedScene>("res://Dice/D20.tscn").Instantiate();
	// 		((Dice)newDice).thrower = Multiplayer.GetUniqueId();
	// 		diceNode.AddChild(newDice, true);
	//
	// 		// Retrieve camera forward
	// 		var cameraHolder = (Node3D)GetViewport().GetCamera3D().GetParent();
	// 		var camera = GetViewport().GetCamera3D();
	//
	// 		var up = camera.GlobalTransform.Basis.Y;
	// 		var left = camera.GlobalTransform.Basis.X;
	// 		
	// 		// Calculate camera position
	// 		if (i % 2 != 0)
	// 		{
	// 			left *= -i;
	// 		}
	// 		
	// 		var cameraPos = cameraHolder.Position + up * -2 + left * 2;
	// 		
	// 		if (newDice is Dice dice)
	// 		{
	// 			dice.throwDice(cameraPos, camera.GlobalTransform.Basis.Z.Normalized() * -1);
	// 		}
	// 		
	// 		diceArray.Add((Node3D)newDice);
	// 	}
	// }
	//
	// private void _increase_dice()
	// {
	// 	numDice++;
	// 	diceLabel.Text = "Num dice: " + numDice;
	// }
	//
	// private void _decrease_dice()
	// {
	// 	if (numDice <= 0) return;
	// 	
	// 	numDice--;
	// 	diceLabel.Text = "Num dice: " + numDice;
	// }
	//
	// private void DeleteDice()
	// {
	// 	startTimer = false;
	// 	currentTime = 0;
	// 	diceRolls = Array.Empty<int>();
	// 	Rpc("_delete_dice", Multiplayer.GetUniqueId());
	// }
	//
	// [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	// private void _delete_dice(int playerId)
	// {
	// 	foreach (var dice in diceArray.Reverse<Node3D>())
	// 	{
	// 		if (((Dice)dice).thrower == playerId)
	// 		{
	// 			diceNode.RemoveChild(dice);
	// 			dice.QueueFree();
	// 			diceArray.Remove(dice);
	// 		}
	// 	}
	// }
}
