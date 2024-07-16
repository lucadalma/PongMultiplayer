using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//classe usata per eseguire i comandi per il movimento del player
public class BarCommandGiver : MonoBehaviour
{
    //mi prendo riferimento del player
    private PongPlayer player;

    //e della barra a lui associatà
    Bar bar = new Bar();

    private void Start()
    {
        //in start mi prendo il component PongPlayer 
        player = NetworkClient.connection.identity.GetComponent<PongPlayer>();

    }

    private void Update()
    {
        //in update mi prendo l'input del player
        if (Input.GetAxis("Vertical") != 0) 
        {
            float yMovement = Input.GetAxis("Vertical");

            Vector3 movementVector = new Vector2(0, yMovement);

            //Eseguo le funzioni di movimento, dando il Vector3 di movimento moltiplicato per la velocità e il deltaTime
            TryMove(movementVector * 10f * Time.deltaTime);

        }
    }

    //funzione di movimento
    private void TryMove(Vector3 position)
    {
        //mi prendo la barra del player
        bar = player.MyBar;

        //richiamo il comando di movimento della classe BarMovement
        bar.BarMovement.CmdMove(position);
    }
}
