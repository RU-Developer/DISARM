using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public PlayerStatus status;
    private PointEffector2D effector;
    void Start()
    {
        StartCoroutine(Explod());
        effector = GetComponent<PointEffector2D>();
        effector.enabled = false;
    }

    IEnumerator Explod()
    {
        yield return new WaitForSeconds(2f);
        effector.enabled = true;
        Managers.Resource.Destroy(this.gameObject,0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            effector.enabled = true;
            Managers.Resource.Destroy(this.gameObject, 0.1f);
        }
    }
}
