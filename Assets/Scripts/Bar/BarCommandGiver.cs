using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BarCommandGiver : MonoBehaviour
{
    private PongPlayer player;

    Bar bar = new Bar();

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<PongPlayer>();

    }

    private void Update()
    {
        if (Input.GetAxis("Vertical") != 0) 
        {
            float yMovement = Input.GetAxis("Vertical");

            Vector3 movementVector = new Vector2(0, yMovement);

            TryMove(movementVector * 10f * Time.deltaTime);

        }
    }

    private void TryMove(Vector3 position)
    {
        bar = player.MyBar;

        bar.BarMovement.CmdMove(position);
    }
}
