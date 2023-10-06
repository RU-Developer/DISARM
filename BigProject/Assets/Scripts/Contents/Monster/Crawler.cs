using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonsterController
{
    public Transform[] movePos;
    [SerializeField] float speed = 1.2f;
    public int moveNum;

    protected override void OnEnable() {
        base.OnEnable();
    }

    private void FixedUpdate()
    {
        if (movePos.Length == 0)
        {
            Destroy(this.gameObject);
        }
        if (moveNum == 0)
        {
            transform.position = movePos[0].position;
            moveNum++;
        }
        if (moveNum!=0 && moveNum < movePos.Length)
        {
            MovePath();
        }
        else if(moveNum>=movePos.Length-1)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
                collision.gameObject.GetComponent<PlayerStatus>().OnDamaged(status);
                Managers.Resource.Destroy(this.gameObject);
        }
    }

    public void MovePath()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePos[moveNum].transform.position,
                                                speed * Time.deltaTime);
        transform.Rotate(new Vector3(0f, 0f, 360f) * Time.deltaTime);
        if (transform.position == movePos[moveNum].transform.position && moveNum != movePos.Length)
            moveNum++;
    }
        
}
