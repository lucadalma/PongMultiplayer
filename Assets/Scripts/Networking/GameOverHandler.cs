using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameOverHandler : NetworkBehaviour
{
    public List<Goal> goals = new List<Goal>();

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer()
    {
        Goal.ServerOnGoalSpawned += ServerHandleGoalSpawned;
        Goal.ServerOnGoalDespawned += ServerHandleGoalDespawned;
    }


    public override void OnStopServer()
    {
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
