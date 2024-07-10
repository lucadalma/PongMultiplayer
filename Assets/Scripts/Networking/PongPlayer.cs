using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PongPlayer : NetworkBehaviour
{
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
        //Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        //Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        //Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        //Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
    //    Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
    //    Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    //    Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
    //    Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    //private void ServerHandleUnitSpawned(Unit unit)
    //{
    //    if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

    //    myUnits.Add(unit);
    //}

    //private void ServerHandleUnitDespawned(Unit unit)
    //{
    //    if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

    //    myUnits.Remove(unit);
    //}

    //private void ServerHandleBuildingSpawned(Building building)
    //{
    //    if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

    //    myBuildings.Add(building);
    //}

    //private void ServerHandleBuildingDespawned(Building building)
    //{
    //    if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

    //    myBuildings.Remove(building);
    //}

    //public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    //{
    //    if (Physics.CheckBox
    //        (point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
    //        return false;


    //    foreach (Building building in myBuildings)
    //    {
    //        if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

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

        //Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        //Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        //Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        //Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopAuthority()
    {
        if (!isClientOnly) return;

        //Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        //Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    //private void AuthorityHandleUnitSpawned(Unit unit)
    //{
    //    myUnits.Add(unit);
    //}

    //private void AuthorityHandleUnitDespawned(Unit unit)
    //{
    //    myUnits.Remove(unit);
    //}

    //private void AuthorityHandleBuildingSpawned(Building building)
    //{
    //    myBuildings.Add(building);
    //}

    //private void AuthorityHandleBuildingDespawned(Building building)
    //{
    //    myBuildings.Remove(building);
    //}

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
