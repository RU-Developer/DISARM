using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private NonDamageableEnvStatus status;

    private void Start()
    {
        status = gameObject.GetComponent<NonDamageableEnvStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStatus player = collision.gameObject.GetComponent<PlayerStatus>();
        if (player != null)
        {
            player.GetComponent<Rigidbody2D>().velocity = transform.up*5f;
            player.OnDamaged(status);
        }
    }
}
