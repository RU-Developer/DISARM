using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public GameObject up, down;
    private bool canLift;
    public bool upward;
    public bool isLift { private set; get; }
    private PlayerController player;
    private void Start()
    {
        isLift = false;
        canLift = true;
    }
    private void FixedUpdate()
    {
        if (!isLift && player != null)
        {
            if (player.transform.position.y < up.transform.position.y + 2 && player.transform.position.y > up.transform.position.y - 2)
            {
                upward = false;
            }
            if (player.transform.position.y < down.transform.position.y + 2 && player.transform.position.y > down.transform.position.y - 2)
            {
                upward = true;
            }
        }
        if(player != null)
        {
            if (isLift)
            {
                if (upward)
                {
                    if (player.transform.position.y > up.transform.position.y)
                    {
                        transform.position = up.transform.position;
                        player.transform.position = new Vector2(up.transform.position.x, up.transform.position.y + 0.8f);
                        isLift = false;
                    }
                }
                else
                {
                    if (player.transform.position.y < down.transform.position.y)
                    {
                        transform.position = down.transform.position;
                        player.transform.position = new Vector2(down.transform.position.x, down.transform.position.y + 0.8f);
                        isLift = false;
                    }
                }
            }
        }
            Lifting();
    }
    private void Lifting()
    {
        if (!isLift) return;

        if (player != null)
        {
            player.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        }
        if (upward)
        {
            transform.Translate(Vector2.up * 5f * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.down * 5f * Time.deltaTime);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            player = collision.gameObject.GetComponent<PlayerController>();
            if (canLift)
            {
                if ((Managers.Input.CurrentAngle < 90 || Managers.Input.CurrentAngle > 270) && upward)
                {
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    isLift = true;
                    canLift = false;
                }
                if ((Managers.Input.CurrentAngle > 90 && Managers.Input.CurrentAngle < 270) && !upward)
                {
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    isLift = true;
                    canLift = false;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        canLift = true;
    }
}
