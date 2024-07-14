using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BarMovement : NetworkBehaviour
{
    float speed = 10f;

    #region Server

    [Server]
    void MovementPlayer(Vector3 position)
    {
        gameObject.transform.Translate(position);

        if (gameObject.transform.position.y >= 3)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, 3);
        }
        else if (gameObject.transform.position.y <= -3)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, -3);
        }

    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        MovementPlayer(position);
    }

    #endregion
}
