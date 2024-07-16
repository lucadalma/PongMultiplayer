using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

//classe per gestire il player
public class PongPlayer : NetworkBehaviour
{
    //ogni player possiede una propria barra e una propria porta
    private Bar myBar = new Bar();

    private Goal myGoal = new Goal();

    public Bar MyBar { get { return myBar; } }

    public Goal MyGoal { get { return myGoal; } }

    //variabili per gestire i punti, il partyowner e il playername
    [SyncVar(hook = nameof(ClientHandlePointUpdated))]
    private int points = 0;
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandlePlayerNameUpdated))]
    private string playerName;

    //all'inizio della partita ad ogni player verrà assegnato un colore random
    private Color playerColour = new Color();
    //getter delle variabili
    public int Points { get { return points; } }
    public Color TeamColour { get { return playerColour; } }
    public bool IsPartyOwner { get { return isPartyOwner; } }
    public string PlayerName { get { return playerName; } }

    //eventi
    public static event Action<int> ClientOnPointUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    #region Server
    public override void OnStartServer()
    {
        //richiamo gli eventi che sono presenti nelle classi Bar e Goal e ci aggancio una funzione del player
        Bar.ServerOnBarSpawned += ServerHandleBarSpawned;
        Bar.ServerOnBarDespawned += ServerHandleBarDespawned;
        Goal.ServerOnGoalSpawned += ServerHandleGoalSpawned;
        Goal.ServerOnGoalDespawned += ServerHandleGoalDespawned;


        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        //sgancio le funzioni dagli eventi
        Bar.ServerOnBarSpawned -= ServerHandleBarSpawned;
        Bar.ServerOnBarDespawned -= ServerHandleBarDespawned;
        Goal.ServerOnGoalSpawned -= ServerHandleGoalSpawned;
        Goal.ServerOnGoalDespawned -= ServerHandleGoalDespawned;
    }

    private void ServerHandleBarSpawned(Bar bar)
    {
        if (bar.connectionToClient.connectionId != connectionToClient.connectionId) return;
        //assengno la barra al player
        myBar = bar;
    }

    private void ServerHandleBarDespawned(Bar bar)
    {
        if (bar.connectionToClient.connectionId != connectionToClient.connectionId) return;
        //annullo l'assegnazione
        myBar = null;
    }

    private void ServerHandleGoalSpawned(Goal goal)
    {
        if (goal.connectionToClient.connectionId != connectionToClient.connectionId) return;
        //stessa cosa per le porte
        myGoal = goal;
    }

    private void ServerHandleGoalDespawned(Goal goal)
    {
        if (goal.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myGoal = null;
    }

    //comando per far partire il game
    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;

        ((PongNetworkManager)NetworkManager.singleton).StartGame();
    }

    //funzione per settare i punti
    [Server]
    public void SetPoints(int amount)
    {
        points = amount;
    }

    //funzione per settare il colore dal player
    [Server]
    public void SetPlayerColour(Color newPlayerColour)
    {
        playerColour = newPlayerColour;
    }

    //funzione per settare il partyOwner
    [Server]
    public void SetIsPartyOwner(bool value)
    {
        isPartyOwner = value;
    }

    //funzione per settare il player Name
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
        //aggiungo a Players all'interno del PongNetworkManager questa classe
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
        //aggancio delle funzioni agli eventi per l'authority del player
        Bar.AuthorityOnBarSpawned += AuthorityHandleBarSpawned;
        Bar.AuthorityOnBarDespawned += AuthorityHandlBarDespawned;
        Goal.AuthorityOnGoalSpawned += AuthorityHandleGoalSpawned;
        Goal.AuthorityOnGoalDespawned += AuthorityHandlGoalDespawned;

    }

    public override void OnStopAuthority()
    {
        if (!isClientOnly) return;

        Bar.AuthorityOnBarSpawned -= AuthorityHandleBarSpawned;
        Bar.AuthorityOnBarDespawned -= AuthorityHandlBarDespawned;
        Goal.AuthorityOnGoalSpawned -= AuthorityHandleGoalSpawned;
        Goal.AuthorityOnGoalDespawned -= AuthorityHandlGoalDespawned;
    }

    private void AuthorityHandleBarSpawned(Bar bar)
    {
        myBar = bar;
    }

    private void AuthorityHandlBarDespawned(Bar bar)
    {
        myBar = null;
    }

    private void AuthorityHandleGoalSpawned(Goal goal)
    {
        myGoal = goal;
    }

    private void AuthorityHandlGoalDespawned(Goal goal)
    {
        myGoal = null;
    }

    //invoco gli eventi creati in precedenza
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!isOwned) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandlePointUpdated(int oldPoint, int newPoint)
    {
        ClientOnPointUpdated?.Invoke(newPoint);
    }

    private void ClientHandlePlayerNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}
