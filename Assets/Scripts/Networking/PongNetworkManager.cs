using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PongNetworkManager : NetworkManager
{
    //Lista dei players presenti in game
    public List<PongPlayer> Players { get; } = new List<PongPlayer>();

    [Space(10)]
    [Header("Pong")]
    //serializzo i gameObject che verranno istanziati una volta che il game partirà
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    //eventi per la gestione della connessione e disconnessione del client
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    //varibile per capire se la partita è in corso
    private bool isGameInProgress = false;
    public bool IsGameInProgress { get { return isGameInProgress; } }

    #region Server

    //quando aggiungo un player al server
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        //aggiungo il player alla lista creata prima
        PongPlayer player = conn.identity.GetComponent<PongPlayer>();
        Players.Add(player);

        //setto il nome del player
        player.SetPlayerName($"Player {Players.Count}");
        //setto il colore
        Color col = new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            );

        player.SetPlayerColour(col);

        //setto il partyowner
        player.SetIsPartyOwner(Players.Count == 1);
    }

    //quando cambio scena e passo dalla scena di menù a quella della partita
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
        {
            //istanzio il gameOverHandler per la gestione della fine partita
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            int i = 0;

            //per ogni player presente in game nel nostro caso 2
            foreach (PongPlayer player in Players)
            {
                //istanzio una barra
                GameObject barInstance = Instantiate(barPrefab, GetStartPosition().position, Quaternion.identity);
                //e gli setto l'owner
                NetworkServer.Spawn(barInstance, player.connectionToClient);

                if (i == 0)
                {
                    //istanzio le porta, ho fatto così per capire in quale lato le porte dovevano essere, perchè ls porta del player per come ho gestito io la cosa,
                    //doveva essere dal lato opposto del player
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
            //istanzio la palla
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
        //disconnessione dal server
        PongPlayer player = conn.identity.GetComponent<PongPlayer>();
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        //se si stoppa il server rimuovo tutti i player
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        //faccio partire la partita solo se ci sono almeno 2 player
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
