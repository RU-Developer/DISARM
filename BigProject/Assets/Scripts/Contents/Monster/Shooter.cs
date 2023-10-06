using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonsterController
{
    private Collider2D detected;
    Rigidbody2D rigid;
    private GameObject bullet;
    private float xspeed, yspeed;
    private bool isEndX, isEndY, canTurn;
    public Vector2 direction;
    private float xturn, yturn;

    protected override void OnEnable()
    {
        base.OnEnable();
        xspeed = 0f;
        yspeed = 0f;
        xturn = 1;
        yturn = 1;
        canTurn = true;
        StartCoroutine(Think());
        rigid = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        detected = Physics2D.OverlapCircle(this.transform.position, 4f, LayerMask.GetMask("Player"));
        isEndX = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(xspeed * xturn), 0.6f, LayerMask.GetMask("Platform"));
        isEndY = Physics2D.Raycast(transform.position, Vector2.up * Mathf.Sign(yspeed * yturn), 0.6f, LayerMask.GetMask("Platform"));
        transform.Translate(new Vector2(xspeed * xturn, yspeed * yturn));

        if (canTurn)
        {
            if (isEndX)
            {
                canTurn = false;
                StartCoroutine(TurnX());
            }
            if (isEndY)
            {
                canTurn = false;
                StartCoroutine(TurnY());
            }
        }
    }
    IEnumerator TurnX()
    {
        xturn = -1;
        yield return new WaitForSeconds(0.5f);
        xturn = 1;
        canTurn = true;
    }
    IEnumerator TurnY()
    {
        yturn = -1;
        yield return new WaitForSeconds(0.5f);
        yturn = 1;
        canTurn = true;
    }
    IEnumerator Think()
    {
        float think = Random.Range(0, 1f);
        xspeed = 0f;
        yspeed = 0f;

        if (detected == null)
        {
            if (think < 0.5f)
            {
                StartCoroutine(Move());
            }
            else
            {
                if (detected != null)
                    StartCoroutine(Detect());
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
        xspeed = Random.Range(-0.01f, 0.01f);
        yspeed = Random.Range(-0.01f, 0.01f);
        if (detected != null)
            StartCoroutine(Detect());
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        StartCoroutine(Think());
    }

    IEnumerator Detect()
    {
        Rigidbody2D player = detected.gameObject.GetComponent<Rigidbody2D>();
        canTurn = false;
        xturn = 1; yturn = 1;
        xspeed = (Mathf.Abs(transform.position.x - player.transform.position.x) >= 0.1f)
            ? Mathf.Sign(player.position.x - transform.position.x) * 0.02f
            : 0;

        yspeed = (Mathf.Abs(transform.position.y - player.transform.position.y) >= 0.4f)
            ? Mathf.Sign(player.position.y - transform.position.y) * 0.005f
            : 0;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        bullet = Managers.Resource.Instantiate("Bullet");
        Managers.Sound.Play("turret_shoot");
        bullet.transform.position = transform.position;
        float angle = Mathf.Atan2(detected.gameObject.transform.position.y - transform.position.y, 
            detected.gameObject.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        yield return new WaitForSeconds(0.5f);
        canTurn = true;
        StartCoroutine(Think());
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            if (collision.contacts[0].point.y >= rigid.position.y + 0.2f)
            {
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
                Die();
            }
            else
            {
                player.GetComponent<PlayerStatus>().OnDamaged(status, transform.right*Mathf.Sign(xspeed) * 10f);
            }
        }
    }

    public override void Die()
    {
        base.Die();
        Managers.Game.Despawn(gameObject);
    }
}
