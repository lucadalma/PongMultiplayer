using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BarMovement : NetworkBehaviour
{
    float speed = 10f;

    #region Server

    [ServerCallback]
    private void Update()
    {
        MovementPlayer();
        LimiteBordiPlayer();
    }

    void LimiteBordiPlayer()
    {
        if (gameObject.transform.position.y >= 3)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, 3);
        }
        else if (gameObject.transform.position.y <= -3)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, -3);
        }

    }


    void MovementPlayer()
    {
        float yMovement = Input.GetAxis("Vertical");

        Vector3 movementVector = new Vector2(0, yMovement);

        gameObject.transform.Translate(movementVector * speed * Time.deltaTime);
    }

    #endregion
}
