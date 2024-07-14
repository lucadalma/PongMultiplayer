using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;


public class Bar : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BarMovement barMovement;

    public BarMovement BarMovement { get { return barMovement; } }

    public static event Action<Bar> ServerOnBarSpawned;
    public static event Action<Bar> ServerOnBarDespawned;

    public static event Action<Bar> AuthorityOnBarSpawned;
    public static event Action<Bar> AuthorityOnBarDespawned;

    #region Server

    public override void OnStartServer()
    {
        ServerOnBarSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBarDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnBarSpawned?.Invoke(this);
    }


    public override void OnStopClient()
    {
        if (!isOwned) return;

        AuthorityOnBarDespawned?.Invoke(this);
    }

    #endregion

}
