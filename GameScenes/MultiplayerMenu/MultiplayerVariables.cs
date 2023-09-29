using Godot;
using System;

public partial class MultiplayerVariables : Node
{
	private int port = 4433;
	private string address;
	private bool isHost;
	private ConnectionEnum connection = ConnectionEnum.NONE;

	public void SetPort(int port)
	{
		this.port = port;
	}

	public int GetPort()
	{
		return port;
	}

	public void SetAddress(string address)
	{
		if (address == "")
		{
			this.address = "localhost";
		}
		else
		{
			this.address = address;
		}
	}

	public string GetAddress()
	{
		return address;
	}

	public void SetHost(bool isHost)
	{
		this.isHost = isHost;
	}

	public bool GetHost()
	{
		return isHost;
	}

	public void SetConnection(ConnectionEnum connection)
	{
		this.connection = connection;
	}

	public ConnectionEnum GetConnection()
	{
		return connection;
	}

	public void Reset()
	{
		address = "";
		isHost = false;
		connection = ConnectionEnum.NONE;
	}
}
