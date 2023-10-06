using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public GameObject lift;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !lift.GetComponent<Lift>().isLift)
        {
            lift.transform.position = new Vector2(lift.transform.position.x,transform.position.y);
        }
    }
}
