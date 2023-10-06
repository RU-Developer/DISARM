using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapper : MonsterController
{
    Collider2D detected;
    Animator animator;
    private float cooldown;
    protected override void OnEnable()
    {
        base.OnEnable();
        cooldown = 0;
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        cooldown -= Time.deltaTime;
        detected = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Player"));

        if (detected != null && cooldown<0)
        {
            cooldown = 4f;
            Managers.Sound.Play("trapper_attack");
            animator.SetBool("isAttack", true);
        }
    }
    void AttackEnd()
    {
        animator.SetBool("isAttack", false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerStatus>().OnDamaged(status);
        }
    }
    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }
}
