using Godot;
using System;

public partial class HandleDiceMenu : Node
{
	[Export] private int _diceRows = 4;
	private DiceContainer[] _diceContainers;
	private DiceEnum[] _countDice;
	private Node _diceHolder;
	
	public override void _Ready()
	{
		var diceContainer = GetNode<HBoxContainer>("Control/PanelContainer/MarginContainer/DiceContainer/ControlDice/MarginContainer/DiceContainer").GetChildren();
		_diceContainers = new DiceContainer[diceContainer.Count];

		for (var i = 0; i < _diceContainers.Length; i++)
		{
			_diceContainers[i] = (DiceContainer)diceContainer[i];
		}
	}

	private void _button_pressed()
	{
		foreach (var container in _diceContainers)
		{
			RollDice(container.DiceEnum, container.GetDiceAmount());
			container.ResetDiceAmount();
		}
	}

	private void RollDice(DiceEnum diceEnum, int diceAmount)
	{
		// Retrieve camera vectors
		var cameraHolder = (Node3D)GetViewport().GetCamera3D().GetParent();
		var camera = GetViewport().GetCamera3D();
	
		var up = camera.GlobalTransform.Basis.Y;
		

		Rpc("_roll_dice",
			diceEnum.ToString(), 
			diceAmount,
			Multiplayer.MultiplayerPeer.GetUniqueId(),
			cameraHolder.Position + up * -2,
			camera.GlobalTransform
			);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void _roll_dice(string diceEnum, int diceAmount, int throwerId, Vector3 initialPosition, Transform3D cameraTransform)
	{
		if (diceAmount == 0) return;
		
		var diceNode = ResourceLoader.Load<PackedScene>("res://Dice/" + diceEnum + "/" + diceEnum + ".tscn").Instantiate();
		var diceHolder = GetNode("/root/Dice/" + Multiplayer.MultiplayerPeer.GetUniqueId());
		
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
}
