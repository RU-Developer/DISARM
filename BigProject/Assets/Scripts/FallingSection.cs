using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingSection : MonoBehaviour
{
    public Vector2 tempSave;
    NonDamageableEnvStatus status;
    private void OnEnable()
    {
        status = GetComponent<NonDamageableEnvStatus>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStatus>().OnDamaged(status);
            collision.transform.position = tempSave;
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

}
