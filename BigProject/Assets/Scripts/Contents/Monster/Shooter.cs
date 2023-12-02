using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonsterController
{
    private Collider2D detected;
    private GameObject bullet;
    private float xspeed, yspeed;
    private bool isEndX, isEndY, canTurn,isShooting,shootRange;
    public Vector2 direction;

    protected override void OnEnable()
    {
        base.OnEnable();
        xspeed = 0f;
        yspeed = 0f;
        isShooting = false;
        canTurn = true;
        StartCoroutine(Think());
    }

    public void FixedUpdate()
    {
        detected = Physics2D.OverlapCircle(this.transform.position, 4f, LayerMask.GetMask("Player"));
        shootRange = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(xspeed), 4f, LayerMask.GetMask("Player"));
        isEndX = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(xspeed), 0.6f, LayerMask.GetMask("Platform"));
        isEndY = Physics2D.Raycast(transform.position, Vector2.up * Mathf.Sign(yspeed), 0.6f, LayerMask.GetMask("Platform"));
        transform.Translate(new Vector2(xspeed, yspeed));
        if (xspeed > 0 && !isShooting)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (xspeed < 0 && !isShooting)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        Debug.Log(canTurn);

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
        xspeed = -xspeed;
        yield return new WaitForSeconds(1f);
        canTurn = true;
    }
    IEnumerator TurnY()
    {
        yspeed = -yspeed;
        yield return new WaitForSeconds(1f);
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
        if (canTurn)
        {
            xspeed = Random.Range(-0.01f, 0.01f);
            yspeed = Random.Range(-0.01f, 0.01f);
        }
        
        if (detected != null)
            StartCoroutine(Detect());
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        StartCoroutine(Think());
    }

    IEnumerator Detect()
    {
        Rigidbody2D player = detected.gameObject.GetComponent<Rigidbody2D>();
        if (canTurn)
        {
            xspeed = (Mathf.Abs(transform.position.x - player.transform.position.x) >= 0.1f)
            ? Mathf.Sign(player.position.x - transform.position.x) * 0.02f
            : 0;

            yspeed = (Mathf.Abs(transform.position.y - player.transform.position.y) >= 0.4f)
                ? Mathf.Sign(player.position.y - transform.position.y) * 0.015f
                : 0;
        }
        
        yield return new WaitForSeconds(1f);
        if (shootRange)
            StartCoroutine(Shoot());
        else
            StartCoroutine(Think());
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        yspeed = 0f;
        bullet = Managers.Resource.Instantiate("Bullet");
        Managers.Sound.Play("turret_shoot");
        bullet.transform.position = transform.position;
        bullet.GetComponent<Bullet>().isEnemy = true;
        if(xspeed>0)
            bullet.transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
        else if(xspeed<0)
            bullet.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        yield return new WaitForSeconds(0.5f);
        canTurn = true;
        isShooting = false;
        StartCoroutine(Think());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerStatus>().OnDamaged(status, transform.right * Mathf.Sign(xspeed) * 10f);
        }
    }

    public override void Die()
    {
        base.Die();
        Managers.Game.Despawn(gameObject);
    }
}
