using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TempSave : MonoBehaviour
{
    public FallingSection fs;
    private void OnEnable()
    {
        this.GetComponent<Tilemap>().color = new Color(0, 0, 0, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            fs.tempSave = collision.transform.position;
        }
    }
}
