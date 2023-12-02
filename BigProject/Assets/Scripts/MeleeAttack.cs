using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private NonDamageableEnvStatus status;
    private bool isReflect = false;
    [HideInInspector]public bool canParry = false;
    public float dir = 0;
    private GameObject projectile;
    private void FixedUpdate()
    {
        if (canParry)
        {
            GetComponent<CircleCollider2D>().enabled = true;
        }
        else
        {
            GetComponent<CircleCollider2D>().enabled = false;
            isReflect = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() != null && !isReflect)
        {
            projectile = Managers.Resource.Instantiate("Bullet");
            isReflect = true;
            Bullet bullet = collision.GetComponent<Bullet>();
            float rotation = bullet.angle;
            projectile.transform.position = collision.gameObject.transform.position;
            projectile.transform.rotation = Quaternion.AngleAxis(rotation + 180 + 30 * dir * Mathf.Sign((int)Managers.Input.CurrentMoveDir), Vector3.forward);
            projectile.GetComponent<Bullet>().isEnemy = false;
            Managers.Resource.Destroy(collision.gameObject);
        }else if(collision.GetComponent<Fire>() !=null && !isReflect)
        {
            projectile = Managers.Resource.Instantiate("Fire");
            projectile.transform.position = collision.gameObject.transform.position;
            isReflect = true;
            Vector2 vector = collision.GetComponent<Rigidbody2D>().velocity * -1;
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(vector.x+10* Mathf.Sign((int)Managers.Input.CurrentMoveDir), vector.y+10*dir);
            projectile.GetComponent<Fire>().isEnemy = false;
            Managers.Resource.Destroy(collision.gameObject);
        }
    }
}
