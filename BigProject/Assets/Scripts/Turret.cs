using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonsterController
{
    private Animator animator;
    private float angle;
    private GameObject bullet;
    private Collider2D detected;
    private Vector2 point;
    private NonDamageableEnvStatus turretStatus;

    protected override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();
        turretStatus = GetComponent<TurretStatus>();
    }
    void FixedUpdate()
    {
        detected = Physics2D.OverlapCircle(transform.position, 5f,LayerMask.GetMask("Player"));

        if (detected !=null)
        {
            point = detected.gameObject.transform.position;
            animator.SetBool("isDetected", true);
            Aim();
        }
        else
        {
            animator.SetBool("isDetected", false);
        }
    }
    void Aim()
    {
        if (!detected) return;
        angle = Mathf.Atan2(point.y - transform.position.y,point.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward * Time.deltaTime);
    }
    void Shoot()
    {
        bullet = Managers.Resource.Instantiate("Bullet");
        Managers.Sound.Play("turret_shoot");
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.turretStatus = turretStatus;
        bulletComp.isEnemy = true;
        bulletComp.angle = angle - 90;
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.AngleAxis(angle-90, Vector3.forward);
    }
}
