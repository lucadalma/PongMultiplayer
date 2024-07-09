using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PongPlayer : NetworkBehaviour
{
    //[SyncVar(hook = nameof(ClientHandlePointUpdated))]
    //private int points = 0;
    //[SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    //private bool isPartyOwner = false;
    //[SyncVar(hook = nameof(ClientHandlePlayerNameUpdated))]
    //private string playerName;

    //private Color playerColour = new Color();

    //public int Points { get { return points; } }
    //public Color TeamColour { get { return playerColour; } }
    //public bool IsPartyOwner { get { return isPartyOwner; } }
    //public string PlayerName { get { return playerName; } }

    //public event Action<int> ClientOnResourceUpdated;
    //public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    //public static event Action ClientOnInfoUpdated;


    //#region Server
    //public override void OnStartServer()
    //{
       

    //    DontDestroyOnLoad(gameObject);
    //}

    //public override void OnStopServer()
    //{

    //}

    //#endregion
}
