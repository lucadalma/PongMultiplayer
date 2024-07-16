using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
//classe per visualizzare i punti in game
public class PointsDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text[] playerPoints = new TMP_Text[2];

    private void OnEnable()
    {
        //mi aggancio all'evento quando il punteggio di un player viene aggiornato
        PongPlayer.ClientOnPointUpdated += ClientHandlePointUpdated;
    }
    

    private void OnDisable()
    {
        //mi sgancio dall'evento
        PongPlayer.ClientOnPointUpdated -= ClientHandlePointUpdated;
    }

    //funzione per aggionare i punteggi in UI
    private void ClientHandlePointUpdated(int  point)
    {
        //mi prendo i player
        List<PongPlayer> players = ((PongNetworkManager)NetworkManager.singleton).Players;

        //aggiorno i punteggi in UI
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
