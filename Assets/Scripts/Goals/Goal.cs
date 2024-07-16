using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

//classe per gestire le porte dei due player
public class Goal : NetworkBehaviour
{
    //mi ottengo il riferimento al player
    public PongPlayer player;

    //eventi che verranno richiamati quando istanzio e distruggo l'oggetto nel server
    public static event Action<Goal> ServerOnGoalSpawned;
    public static event Action<Goal> ServerOnGoalDespawned;

    //eventi per gestire l'autoritò del player
    public static event Action<Goal> AuthorityOnGoalSpawned;
    public static event Action<Goal> AuthorityOnGoalDespawned;

    public override void OnStartServer()
    {
        //riferimento al player
        player = connectionToClient.identity.GetComponent<PongPlayer>();

        //invoco l'evento di prima
        ServerOnGoalSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        //anche qui come nella classe Bar, uso questi eventi per gestire la spawn e despawn degli oggetti
        ServerOnGoalDespawned?.Invoke(this);
    }

    //gestisco la collisione con la palla
    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            if (player == null) return;
            //aggiungo un punto al player se fa gol, ogni player corrisponde una porta nel lato opposto, se fa colpisce quella porta fa punto
            player.SetPoints(player.Points + 1);
            Debug.Log("Goal: " + player.connectionToClient.connectionId + " Points: " + player.Points);
        }
    }

    private void Update()
    {
        //controllo la vittoria
        ServerHandleWin();
    }

    [Server]
    private void ServerHandleWin() 
    {
        //se il player arriva a 5 punti vince
        if (player.Points == 5)
        {
            NetworkServer.Destroy(gameObject);

        }
    }

    public override void OnStartAuthority()
    {
        AuthorityOnGoalSpawned?.Invoke(this);
    }


    public override void OnStopClient()
    {
        if (!isOwned) return;

        AuthorityOnGoalDespawned?.Invoke(this);
    }
}
