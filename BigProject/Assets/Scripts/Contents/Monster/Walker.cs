using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonsterController
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator animator;
    private bool isEnd, isWall, isGrounded, detected,canAttack,isAttack;
    private float dir, xspeed;
    private float initMass;

    protected override void OnEnable()
    {
        base.OnEnable();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        dir = 1f;
        xspeed = 0;
        canAttack = true;
        initMass = rigid.mass;
        StartCoroutine(Idle());
    }

    public void FixedUpdate()
    {
        detected = Physics2D.Raycast(transform.position, Vector2.right * dir, 3f, LayerMask.GetMask("Player"));
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));
        isWall = Physics2D.Raycast(rigid.position, Vector2.right * dir, 0.3f, LayerMask.GetMask("Platform"));
        isEnd = !(Physics2D.Raycast(rigid.position + new Vector2(0.3f * dir, 0), Vector2.down, 0.6f, LayerMask.GetMask("Platform")));
        transform.Translate(new Vector2(dir * xspeed, 0));
        

        if (dir > 0)
        {
            sprite.flipX = false;
        }
        else if (dir < 0)
        {
            sprite.flipX = true;
        }
    }

    IEnumerator Think()
    {
        xspeed = 0;
        animator.speed = 1;
        if (isGrounded) rigid.velocity = Vector2.zero;
        float think = Random.Range(0f, 2f);
        yield return null;
        if (detected && canAttack)
        {
            canAttack = false;
            animator.SetBool("isIdle", false);
            animator.SetBool("isMove", false);
            animator.SetBool("isAttack", true);
        }
        else
        {
            if (think < 1)
            {
                StartCoroutine(Move());
            }
            else
            {
                StartCoroutine(Idle());
            }
        }
    }

    IEnumerator Move()
    {
        dir = Mathf.Sign(Random.Range(-1, 1));
        xspeed = 0.03f;
        animator.SetBool("isMove", true);
        if (isGrounded && (isWall || isEnd))
        {
            dir *= -1;
        }
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isMove", false);
        StartCoroutine(Think());
    }

    IEnumerator Idle()
    {
        animator.SetBool("isIdle", true);
        if (isGrounded && (isWall || isEnd))
        {
            dir *= -1;
        }
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isIdle", false);
        StartCoroutine(Think());
    }

    IEnumerator Attack()
    {
        xspeed = 0.08f;
        isAttack = true;
        Managers.Sound.Play("slime_attack");
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isAttack", false);
        StartCoroutine(Think());
        yield return new WaitForSeconds(2f);
        canAttack = true;
        isAttack = false;
        xspeed = 0.03f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            rigid.mass = 100;
            if (collision.contacts[0].point.y+0.1f >= rigid.position.y)
            {
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 18f, ForceMode2D.Impulse);
                status.OnDamaged(player.GetComponent<PlayerStatus>());
            }
            else
            {
                if (isAttack)
                    player.GetComponent<PlayerStatus>().OnDamaged(status, new Vector2(dir * 10f, 0), status.Attack);
            }
        }
        else
        {
            rigid.mass = initMass;
        }
    }

    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }
}
