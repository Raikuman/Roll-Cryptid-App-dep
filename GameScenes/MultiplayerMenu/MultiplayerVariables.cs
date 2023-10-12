using Godot;
using System;

public partial class MultiplayerVariables : Node
{
	private int _port = 4433;
	private string _address;
	private bool _isHost;
	private ConnectionEnum _connection = ConnectionEnum.NONE;

	public void SetPort(int port)
	{
		this._port = port;
	}

	public int GetPort()
	{
		return _port;
	}

	public void SetAddress(string address)
	{
		if (address == "")
		{
			this._address = "localhost";
		}
		else
		{
			this._address = address;
		}
	}

	public string GetAddress()
	{
		return _address;
	}

	public void SetHost(bool isHost)
	{
		this._isHost = isHost;
	}

	public bool GetHost()
	{
		return _isHost;
	}

	public void SetConnection(ConnectionEnum connection)
	{
		this._connection = connection;
	}

	public ConnectionEnum GetConnection()
	{
		return _connection;
	}

	public void Reset()
	{
		_address = "";
		_isHost = false;
		_connection = ConnectionEnum.NONE;
	}
}
