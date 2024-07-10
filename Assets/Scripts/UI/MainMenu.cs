using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private bool usingSteam = false;

    private Callback<LobbyCreated_t> lobbyCreated;
    private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    private Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        if (!usingSteam) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (usingSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyCreated(LobbyCreated_t callBack)
    {
        if (callBack.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            return;
        }

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callBack.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callBack)
    {
        SteamMatchmaking.JoinLobby(callBack.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callBack)
    {
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callBack.m_ulSteamIDLobby),
            "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        landingPagePanel.SetActive(false);
    }


}
