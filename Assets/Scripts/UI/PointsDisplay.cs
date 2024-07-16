using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class PointsDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text[] playerPoints = new TMP_Text[2];

    private void OnEnable()
    {
        PongPlayer.ClientOnPointUpdated += ClientHandlePointUpdated;
    }
    

    private void OnDisable()
    {

        PongPlayer.ClientOnPointUpdated -= ClientHandlePointUpdated;
    }
    private void ClientHandlePointUpdated(int  point)
    {
        List<PongPlayer> players = ((PongNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            playerPoints[i].text = players[i].Points.ToString();
        }

        for (int i = players.Count; i < playerPoints.Length; i++)
        {
            playerPoints[i].text = "0";
        }
    }
}
