using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform[] movePos;
    [SerializeField] float speed = 1.2f;
    int moveNum = 0;
    private bool reverse = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = movePos[moveNum].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MovePath();
    }

    public void MovePath()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePos[moveNum].transform.position,
                                                speed * Time.deltaTime);
        if (!reverse && transform.position == movePos[moveNum].transform.position)
            moveNum++;
        if (reverse && transform.position == movePos[moveNum].transform.position)
            moveNum--;
        if (moveNum == movePos.Length)
        {
            moveNum = movePos.Length - 1;
            reverse = true;
        }
        else if (moveNum == 0)
        {
            moveNum = 0;
            reverse = false;
        }
    }
}
