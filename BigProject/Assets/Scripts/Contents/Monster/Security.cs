using System.Collections;
using UnityEngine;
public class Security : MonsterController
{
    private Collider2D detected;
    private Collider2D attack;
    private bool isGrounded, isWall, isEnd;
    private float dir,xspeed,initMass;
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Think());
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        xspeed = 0f;
        initMass = rigid.mass;
    }

    private void FixedUpdate()
    {
        detected = Physics2D.OverlapCircle(transform.position, 5f, LayerMask.GetMask("Player"));
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));
        isWall = Physics2D.Raycast(rigid.position, Vector2.right * dir, 1f, LayerMask.GetMask("Platform","Path"));
        isEnd = !(Physics2D.Raycast(rigid.position + new Vector2(0.3f * dir, 0), Vector2.down, 0.6f, LayerMask.GetMask("Platform")));
        
        transform.Translate(new Vector2(dir * xspeed, 0));

        if (isGrounded && (isWall || isEnd))
        {
            dir *= -1;
        }

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
        float think = Random.Range(0, 1f);
        xspeed = 0;
        if (detected == null)
        {
            if (think < 0.6f)
            {
                StartCoroutine(Move());
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                StartCoroutine(Think());
            }
        }
        else
        {
            StartCoroutine(Detect());
        }
        
    }

    IEnumerator Move()
    {
        dir = Mathf.Sign(Random.Range(-1f, 1f));
        xspeed = 0.05f;
        
        yield return new WaitForSeconds(Random.Range(0.5f,2f));
        StartCoroutine(Think());
    }

    IEnumerator Detect()
    {
        dir = Mathf.Sign(detected.gameObject.transform.position.x - transform.position.x);
        xspeed = 0.05f;

        attack = Physics2D.OverlapBox(transform.position, new Vector2(1f*dir, 0.5f),0f,LayerMask.GetMask("Player"));
        if(attack != null)
        {
            StartCoroutine(Attack());
        }

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(Think());
    }
    IEnumerator Attack()
    {
        detected.gameObject.GetComponent<PlayerStatus>().OnDamaged(status,Vector2.right*dir*20f);
        yield return null;
    }

    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            rigid.mass = 100;
            if (collision.contacts[0].point.y >= rigid.position.y)
            {
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 12f, ForceMode2D.Impulse);
            }
        }
        else
        {
            rigid.mass = initMass;
        }
    }
}