using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

//classe generale per la barra
public class Bar : NetworkBehaviour
{
    //prendo un riferimento allo script del movement
    [Header("References")]
    [SerializeField] private BarMovement barMovement;

    public BarMovement BarMovement { get { return barMovement; } }

    //Eventi per quando istanzio e e tolgo la barra nella scena
    public static event Action<Bar> ServerOnBarSpawned;
    public static event Action<Bar> ServerOnBarDespawned;

    //eventi per gestire quale player ha autorità su quale barra
    public static event Action<Bar> AuthorityOnBarSpawned;
    public static event Action<Bar> AuthorityOnBarDespawned;

    #region Server

    //appena istanzio la barra
    public override void OnStartServer()
    {
        //richiamo l'evento di prima che quindi verrà invocato ogni volta che partirà l'OnStartServer
        ServerOnBarSpawned?.Invoke(this);
    }

    //appena viene distutta la barra
    public override void OnStopServer()
    {
        //richiamo anche qua l'evento
        ServerOnBarDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    //Stessa cosa di prima, semplicemente qua viene richiamato il codice per gli oggetti sui quali il client ha autorità 
    public override void OnStartAuthority()
    {
        AuthorityOnBarSpawned?.Invoke(this);
    }

    //stessa cosa solo che in OnStopClient e faccio il controllo (isOwned) per capire se l'oggetto è posseduto dal player
    public override void OnStopClient()
    {
        if (!isOwned) return;

        AuthorityOnBarDespawned?.Invoke(this);
    }

    #endregion

}
