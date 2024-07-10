using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private Button joinButton;

    private void OnEnable()
    {
        PongNetworkManager.ClientOnConnected += HandleClientConnected;
        PongNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        PongNetworkManager.ClientOnConnected -= HandleClientConnected;
        PongNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
