using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//classe che muove effettivamente la barra
public class BarMovement : NetworkBehaviour
{
    #region Server

    //con questa funzione che viene alla fine richiamata nello script BarCommandGiver, muovo il player
    [Server]
    void MovementPlayer(Vector3 position)
    {
        //traslo la posizione in base al Vector3 nello script BarCommandGiver
        gameObject.transform.Translate(position);

        //aggiungo dei limmiti alle barre per non farle andare troppo oltre i limiti della mappa
        if (gameObject.transform.position.y >= 3)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, 3);
        }
        else if (gameObject.transform.position.y <= -3)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, -3);
        }

    }

    //comando per muovere il player richiamato dallo script BarCommandGiver
    [Command]
    public void CmdMove(Vector3 position)
    {
        MovementPlayer(position);
    }

    #endregion
}
