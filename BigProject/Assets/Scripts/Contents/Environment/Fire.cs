using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : ProjectileStatus
{
    private NonDamageableEnvStatus status;
    public bool isEnemy;

    protected override void OnEnable()
    {
        base.OnEnable();
        status = GetComponent<NonDamageableEnvStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemy)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerStatus>().OnDamaged(status);
                Managers.Resource.Destroy(this.gameObject);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Monster")
            {
                if (collision.gameObject.GetComponent<MonsterStatus>() != null)
                {
                    collision.gameObject.GetComponent<MonsterStatus>().OnDamaged(
                    Managers.Game.GetPlayer().GetComponent<PlayerStatus>(),Vector2.zero,20);
                }
                Managers.Resource.Destroy(this.gameObject);
            }
        }


        if (collision.gameObject.tag == "Platform")
        {
            Managers.Resource.Destroy(this.gameObject);
        }
    }
}
