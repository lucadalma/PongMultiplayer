using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//classe per gestire la fine partita
public class GameOverHandler : NetworkBehaviour
{
    public List<Goal> goals = new List<Goal>();

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer()
    {
        //mi aggancio agli eventi presenti nella classe goal
        Goal.ServerOnGoalSpawned += ServerHandleGoalSpawned;
        Goal.ServerOnGoalDespawned += ServerHandleGoalDespawned;
    }


    public override void OnStopServer()
    {
        //mi sgancio dagli eventi
        Goal.ServerOnGoalSpawned -= ServerHandleGoalSpawned;
        Goal.ServerOnGoalDespawned -= ServerHandleGoalDespawned;
    }

    [Server]
    private void ServerHandleGoalSpawned(Goal goal)
    {
        goals.Add(goal);
    }

    [Server]
    private void ServerHandleGoalDespawned(Goal goal)
    {
        //quando despawuno la porta (qunado il player arriva a 5 punti), la partita termina e faccio visualizzare il vincitore
        goals.Remove(goal);

        if (goals.Count != 1) return;

        int playerId = goal.connectionToClient.connectionId;

        RpcGameOver($"Player {playerId}");
        ServerOnGameOver?.Invoke();
    }


    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
