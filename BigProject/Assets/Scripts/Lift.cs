using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public GameObject up, down;
    private bool upward,canLift;
    public bool isLift { private set; get; }
    private PlayerController player;
    private Animator animator;
    private void Start()
    {
        isLift = false;
        canLift = true;
    }
    private void FixedUpdate()
    {
        if (!upward && transform.position.y <= down.transform.position.y)
        {
            if (player != null) player.isFix = false;
            isLift = false;
            upward = true;
        }
        else if (upward && transform.position.y >= up.transform.position.y)
        {
            if (player != null) player.isFix = false;
            isLift = false;
            upward = false;
        }
        Lifting();
    }
    private void Lifting()
    {
        if (!isLift) return;

        if (upward)
        {
            transform.Translate(Vector2.up * 5f * Time.deltaTime);
            if (player != null)
            {
                player.transform.Translate(Vector2.up * 5f * Time.deltaTime);
            }
        }
        else
        {
            transform.Translate(Vector2.down * 5f * Time.deltaTime);
            if (player != null)
            {
                player.transform.Translate(Vector2.down * 5f * Time.deltaTime);
            }
        }

        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && canLift)
        {
            player = collision.gameObject.GetComponent<PlayerController>();
            animator = collision.gameObject.GetComponent<Animator>();
            if (!player.isFix)
            {
                if (upward&&animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.player_idle_lookup"))
                {
                    player.isFix = true;
                    player.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    isLift = true;
                    canLift = false;
                }
                if (!upward&&animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.player_idle_lookdown"))
                {
                    player.isFix = true;
                    player.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    isLift = true;
                    canLift = false;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canLift = true;
        }
    }
}
