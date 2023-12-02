using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileStatus
{
    public NonDamageableEnvStatus turretStatus;
    public MonsterStatus monsterStatus;
    public bool isEnemy;
    public float angle;
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    void FixedUpdate()
    {
        transform.Translate(Vector2.up * 8f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemy)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerStatus>().OnDamaged(turretStatus, Vector2.zero, 10);
                Managers.Resource.Destroy(this.gameObject);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Monster")
            {
                TurretStatus turret = collision.GetComponent<TurretStatus>();
                if (turret != null)
                {
                    turret.OnDamaged(Managers.Game.GetPlayer().GetComponent<PlayerStatus>(), Vector2.zero, 10);
                    Managers.Resource.Destroy(this.gameObject);
                    return;
                }

                if (collision.GetComponent<MonsterStatus>() != null)
                {
                    collision.gameObject.GetComponent<MonsterStatus>().OnDamaged(
                    Managers.Game.GetPlayer().GetComponent<PlayerStatus>(), Vector2.zero, 10);
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
