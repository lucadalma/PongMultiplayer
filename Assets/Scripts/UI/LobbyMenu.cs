using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];

    private void OnEnable()
    {
        PongNetworkManager.ClientOnConnected += HandleClientConnected;
        PongPlayer.AuthorityOnPartyOwnerStateUpdated += AutorityHandlePartyOwnerStateUpdated;
        PongPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDisable()
    {
        PongNetworkManager.ClientOnConnected -= HandleClientConnected;
        PongPlayer.AuthorityOnPartyOwnerStateUpdated -= AutorityHandlePartyOwnerStateUpdated;
        PongPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

    private void AutorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<PongPlayer>().CmdStartGame();
    }

    private void ClientHandleInfoUpdated()
    {
        List<PongPlayer> players = ((PongNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].PlayerName;
        }

        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for player...";
        }

        startGameButton.interactable = players.Count == 2;
    }
}
