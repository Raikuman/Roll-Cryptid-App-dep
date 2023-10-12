using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HandleDiceMenu : Node
{
	[Export] private int _diceRows = 4;
	private DiceContainer[] _diceContainers;
	private string[] _diceNames;
	private Node _diceHolder;
	private Timer _timer;
	private bool _checkForRoll;
	
	public override void _Ready()
	{
		var diceContainer = GetNode<HBoxContainer>("Control/PanelContainer/MarginContainer/DiceContainer/ControlDice/MarginContainer/DiceContainer").GetChildren();
		_diceContainers = new DiceContainer[diceContainer.Count];

		for (var i = 0; i < _diceContainers.Length; i++)
		{
			_diceContainers[i] = (DiceContainer)diceContainer[i];
		}
		
		_diceHolder = GetNode("/root/Game/Dice");
		_timer = GetNode<Timer>("Timer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_checkForRoll) return;
		
		if (!CheckCompleteRolls()) return;

		RetrieveRolls();
	}

	private void _button_pressed()
	{
		var names = new List<string>();
		foreach (var container in _diceContainers)
		{
			names.AddRange(RollDice(container.DiceEnum, container.GetDiceAmount()));
			container.ResetDiceAmount();
		}

		_diceNames = names.ToArray();
		_checkForRoll = true;
	}

	private void _timer_timeout()
	{
		// Delete dice
		foreach (var diceName in _diceNames)
		{
			Rpc("_delete_dice", diceName);
		}
		
		_diceNames = Array.Empty<string>();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _delete_dice(string diceName)
	{
		GetNode("/root/Game/Dice/" + diceName).QueueFree();
	}

	private void RetrieveRolls()
	{
		var rollDictionary = new Godot.Collections.Dictionary();
		foreach (var diceName in _diceNames)
		{
			var dice = _diceHolder.GetNode<Dice>(diceName);

			if (!rollDictionary.ContainsKey(dice.DiceEnum))
			{
				rollDictionary.Add(dice.DiceEnum, new Godot.Collections.Array{dice.UpmostFace});
			}
			else
			{
				rollDictionary[dice.DiceEnum].AsGodotArray<int>().Add(dice.UpmostFace);
			}
		}

		var rollString = "";
		var total = 0;
		var last = rollDictionary.Last();
		foreach (var value in rollDictionary)
		{
			var rollArray = value.Value.AsGodotArray<int>();
			rollString += "[" + rollArray.Count + value.Key.ToString().ToLower() + ": ";
			
			for (var i = 0; i < rollArray.Count; i++)
			{
				rollString += rollArray[i];
				total += rollArray[i];
				
				// Handle commas between rolls
				if (i < rollArray.Count - 1) rollString += ", ";
			}

			rollString += "]";
			
			// Handle space between roll groups
			if (!value.Equals(last)) rollString += " ";
		}
		
		// Create log
		GetNode<ChatHandler>("/root/Game/ChatHandler").CreateLog(
			GetNode<PlayerVariables>("/root/PlayerVariables").GetUsername() + " rolled: " + total + " " + rollString);

		_timer.Start();
		_checkForRoll = false;
	}

	private bool CheckCompleteRolls()
	{
		foreach (var diceName in _diceNames)
		{
			if (!_diceHolder.GetNode<Dice>(diceName).CompletedRoll) return false;
		}

		return true;
	}

	private IEnumerable<string> RollDice(DiceEnum diceEnum, int diceAmount)
	{
		if (diceAmount == 0) return new List<string>();
		var names = new List<string>();
		
		// Retrieve camera vectors
		var cameraHolder = (Node3D)GetViewport().GetCamera3D().GetParent();
		var camera = GetViewport().GetCamera3D();
	
		var up = camera.GlobalTransform.Basis.Y;
		var initialPosition = cameraHolder.Position + up * -2;
		var cameraTransform = camera.GlobalTransform;

		var left = cameraTransform.Basis.X * 2;
		var backward = cameraTransform.Basis.Z * 2;
		var down = cameraTransform.Basis.Y * -2;
		
		var diceInRow = 0;
		var rows = 0;
		var set = 0;
		for (var i = 0; i < diceAmount; i++)
		{
			// Setup position
			var spawnPos = initialPosition;
			var rowPos = left;
	
			// Create new set
			spawnPos += down * set;
			
			// Create new rows
			spawnPos += backward * (rows + set);
			
			// Alternate row position
			if (i % 2 != 0) rowPos *= -1;
	
			// Set row position
			spawnPos += rowPos * diceInRow * 0.5f;

			// Setup name
			var name = diceEnum + "|" + Multiplayer.MultiplayerPeer.GetUniqueId() + "|" + i;
			names.Add(name);
			
			// RPC Dice
			Rpc("_add_dice",
				diceEnum.ToString(),
				name,
				Multiplayer.MultiplayerPeer.GetUniqueId(),
				spawnPos,
				cameraTransform.Basis.Z.Normalized() * -1);
			
			// Update rows and sets
			if (i > 0 && i % _diceRows == 0)
			{
				diceInRow = 0;
				rows++;
			}
	
			if (rows >= _diceRows)
			{
				diceInRow = 0;
				rows = 0;
				set++;
			}
	
			diceInRow++;
		}

		return names;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _roll_dice(string diceName, string diceEnum, int diceAmount, int throwerId, Vector3 initialPosition, Transform3D cameraTransform)
	{
		if (diceAmount == 0) return;
		
		var diceNode = ResourceLoader.Load<PackedScene>("res://Dice/" + diceEnum + "/" + diceEnum + ".tscn").Instantiate();
		var diceHolder = GetNode("/root/Game/Dice");
		
		var left = cameraTransform.Basis.X * 2;
		var backward = cameraTransform.Basis.Z * 2;
		var down = cameraTransform.Basis.Y * -2;
	
		var diceInRow = 0;
		var rows = 0;
		var set = 0;
		for (var i = 0; i < diceAmount; i++)
		{
			var newDice = diceNode.Duplicate();
			((Dice)newDice).ThrowerId = throwerId;
			((Dice)newDice).ThrowPosition = initialPosition + left;
			newDice.Name = "D20-" + i;
			
			// Setup position
			var spawnPos = initialPosition;
			var rowPos = left;
	
			// Create new set
			spawnPos += down * set;
			
			// Create new rows
			spawnPos += backward * (rows + set);
			
			// Alternate row position
			if (i % 2 != 0) rowPos *= -1;
	
			// Set row position
			spawnPos += rowPos * diceInRow * 0.5f;
			
			diceHolder.AddChild(newDice, true);
			
			// Throw dice
			if (newDice is Dice dice)
			{
				dice.Name = diceName;
				dice.ThrowerId = throwerId;
				dice.ThrowPosition = spawnPos;
				dice.ThrowAngle = cameraTransform.Basis.Z.Normalized() * -1;
				dice.ThrowDice();
			}
			
			// Update rows and sets
			if (i > 0 && i % _diceRows == 0)
			{
				diceInRow = 0;
				rows++;
			}
	
			if (rows >= _diceRows)
			{
				diceInRow = 0;
				rows = 0;
				set++;
			}
	
			diceInRow++;
		}
		
		diceNode.QueueFree();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _add_dice(string diceEnum, string name, int throwerId, Vector3 throwPosition, Vector3 throwAngle)
	{
		var diceNode = ResourceLoader.Load<PackedScene>("res://Dice/" + diceEnum + "/" + diceEnum + ".tscn").Instantiate();

		if (diceNode is not Dice dice) return;
		_diceHolder.AddChild(diceNode);
		dice.Name = name;
		dice.DiceEnum = diceEnum;
		dice.ThrowerId = throwerId;
		dice.ThrowPosition = throwPosition;
		dice.ThrowAngle = throwAngle;
		dice.ThrowDice();
	}
}
