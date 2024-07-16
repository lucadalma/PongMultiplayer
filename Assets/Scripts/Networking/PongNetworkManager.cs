using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PongNetworkManager : NetworkManager
{
    public List<PongPlayer> Players { get; } = new List<PongPlayer>();

    [Space(10)]
    [Header("Pong")]
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress = false;
    public bool IsGameInProgress { get { return isGameInProgress; } }

    #region Server

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PongPlayer player = conn.identity.GetComponent<PongPlayer>();
        Players.Add(player);

        player.SetPlayerName($"Player {Players.Count}");

        Color col = new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            );

        player.SetPlayerColour(col);


        player.SetIsPartyOwner(Players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            int i = 0;

            foreach (PongPlayer player in Players)
            {
                
                GameObject barInstance = Instantiate(barPrefab, GetStartPosition().position, Quaternion.identity);
                NetworkServer.Spawn(barInstance, player.connectionToClient);

                if (i == 0)
                {
                    GameObject goalInstance = Instantiate(goalPrefab, new Vector3(9.8f, 0, 0), Quaternion.identity);
                    NetworkServer.Spawn(goalInstance, player.connectionToClient);
                }
                else 
                {
                    GameObject goalInstance = Instantiate(goalPrefab, new Vector3(-9.8f, 0, 0), Quaternion.identity);
                    NetworkServer.Spawn(goalInstance, player.connectionToClient);
                }

                i++;

            }

            GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(ball);

        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!isGameInProgress) return;

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        PongPlayer player = conn.identity.GetComponent<PongPlayer>();
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count < 2) return;

        isGameInProgress = true;

        ServerChangeScene("Map_Level");
    }

    #endregion


    #region Client
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}
