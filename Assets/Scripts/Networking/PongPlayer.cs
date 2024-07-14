using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PongPlayer : NetworkBehaviour
{
    private Bar myBar = new Bar();

    public Bar MyBar { get { return myBar; } }

    [SyncVar(hook = nameof(ClientHandlePointUpdated))]
    private int points = 0;
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandlePlayerNameUpdated))]
    private string playerName;

    private Color playerColour = new Color();

    public int Points { get { return points; } }
    public Color TeamColour { get { return playerColour; } }
    public bool IsPartyOwner { get { return isPartyOwner; } }
    public string PlayerName { get { return playerName; } }

    public event Action<int> ClientOnResourceUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    #region Server
    public override void OnStartServer()
    {
        Bar.ServerOnBarSpawned += ServerHandleBarSpawned;
        Bar.ServerOnBarDespawned += ServerHandleBarDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Bar.ServerOnBarSpawned -= ServerHandleBarSpawned;
        Bar.ServerOnBarDespawned -= ServerHandleBarDespawned;
    }

    private void ServerHandleBarSpawned(Bar bar)
    {
        if (bar.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myBar = bar;
    }

    private void ServerHandleBarDespawned(Bar bar)
    {
        if (bar.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myBar = null;
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;

        ((PongNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Server]
    public void SetPoints(int amount)
    {
        points = amount;
    }

    [Server]
    public void SetPlayerColour(Color newPlayerColour)
    {
        playerColour = newPlayerColour;
    }

    [Server]
    public void SetIsPartyOwner(bool value)
    {
        isPartyOwner = value;
    }

    [Server]
    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    #endregion

    #region Client


    public override void OnStartClient()
    {
        if (NetworkServer.active) return;

        ((PongNetworkManager)NetworkManager.singleton).Players.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) return;
        ((PongNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;

        Bar.AuthorityOnBarSpawned += AuthorityHandleBarSpawned;
        Bar.AuthorityOnBarDespawned += AuthorityHandlBarDespawned;

    }

    public override void OnStopAuthority()
    {
        if (!isClientOnly) return;

        Bar.AuthorityOnBarSpawned -= AuthorityHandleBarSpawned;
        Bar.AuthorityOnBarDespawned -= AuthorityHandlBarDespawned;
    }

    private void AuthorityHandleBarSpawned(Bar bar)
    {
        myBar = bar;
    }

    private void AuthorityHandlBarDespawned(Bar bar)
    {
        myBar = null;
    }


    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!isOwned) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandlePointUpdated(int oldPoint, int newPoint)
    {
        ClientOnResourceUpdated?.Invoke(newPoint);
    }

    private void ClientHandlePlayerNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
