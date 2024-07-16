using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Goal : NetworkBehaviour
{

    public PongPlayer player;

    public static event Action<Goal> ServerOnGoalSpawned;
    public static event Action<Goal> ServerOnGoalDespawned;

    public static event Action<Goal> AuthorityOnGoalSpawned;
    public static event Action<Goal> AuthorityOnGoalDespawned;

    public override void OnStartServer()
    {
        player = connectionToClient.identity.GetComponent<PongPlayer>();

        ServerOnGoalSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnGoalDespawned?.Invoke(this);
    }

    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            if (player == null) return;
            player.SetPoints(player.Points + 1);
            Debug.Log("Goal: " + player.connectionToClient.connectionId + " Points: " + player.Points);
        }
    }

    private void Update()
    {
        ServerHandleWin();
    }

    [Server]
    private void ServerHandleWin() 
    {
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
