using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//script che gestisce la palla
public class BallScript : NetworkBehaviour
{
    //prendo il riferimento al rigidbody
    [SerializeField] private Rigidbody2D rb;
    //creo una variabile per la forza di spinta iniziale
    [SerializeField] int force = 10;
    //creo una variabile per accellerare la palla ogni volta che colpisce un player
    [SerializeField] float accelerationFactor = 1.1f;

    public int Force { get { return force; } }

    void Start()
    {
        //appena istanzio la palla la lacio verso una direzione a caso usando questa funzione
        LaunchBall();
    }

    //funzione per resettare la palla quando uno dei due player fa goal
    public void ResetBall()
    {
        //resetto la posizione e la velocità 
        transform.position = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    //funzione per lanciare la palla in una direzione random
    public void LaunchBall()
    {
        //prendo le due direzioni, verticali e orrizzontali e le do in pasto a un vector 2 che mi darà la direzione generale
        float horizontalDirection = Mathf.Sign(Random.Range(-100f, 100f));
        float verticalDirection = Random.Range(-1f, 1f);

        Vector2 direction = new Vector2(horizontalDirection, verticalDirection);

        // applico la forza alla palla moltiplicando la direzione per la forza di spinta
        rb.AddForce(direction * force);
    }

    //gestisco le collisioni 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // se colpisco una porta resetto la palla e la lancio
        if (collision.transform.CompareTag("Goal"))
        {

            ResetBall();
            LaunchBall();
        }

        // se colpisco un player
        if (collision.transform.CompareTag("BarPlayer"))
        {
            //mi prendo la velocità attuale
            Vector2 currentVelocity = rb.velocity;

            //la moltiplico per l'accelerazione che voglio
            Vector2 newVelocity = currentVelocity * accelerationFactor;
            //setto la nuova velocità
            rb.velocity = newVelocity;
        }
    }
}
