using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallScript : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] int force = 10;

    public int Force { get { return force; } }


    void Start()
    {
        LaunchBall();
    }

    public void ResetBall()
    {
        transform.position = Vector2.zero;
        rb.velocity = Vector2.zero;
    }

    public void LaunchBall()
    {
        float horizontalDirection = Mathf.Sign(Random.Range(-100f, 100f));
        float verticalDirection = Random.Range(-1f, 1f);

        Vector2 direction = new Vector2(horizontalDirection, verticalDirection);

        rb.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("GoalP1"))
        {
            ResetBall();
            LaunchBall();
        }
        else if (collision.transform.CompareTag("GoalP2"))
        {
            ResetBall();
            LaunchBall();
        }

        if (collision.transform.CompareTag("BarPlayer"))
        {
            rb.velocity += new Vector2(2,0);
        }
    }
}
