using Godot;
using System;

public partial class PlayerVariables : Node
{
    private bool _enableCamera = true;
    private bool _pauseUI;
    private string _username = "";

    public void CameraEnable(bool enable)
    {
        _enableCamera = enable;
    }

    public bool GetCameraEnabled()
    {
        return _enableCamera;
    }

    public void SetUsername(string username)
    {
        _username = username;
    }

    public string GetUsername()
    {
        if (string.IsNullOrEmpty(_username))
        {
            return "Player" + Multiplayer.MultiplayerPeer.GetUniqueId();
        }
        else
        {
            return _username;
        }
    }

    public void PauseUI(bool pause)
    {
        _pauseUI = pause;
    }

    public bool GetPauseUI()
    {
        return _pauseUI;
    }
}
