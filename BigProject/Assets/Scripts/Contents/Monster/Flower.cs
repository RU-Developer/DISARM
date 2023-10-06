using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonsterController
{
    private GameObject fire;
    Collider2D detected;
    private bool canFire;
    protected override void OnEnable()
    {
        base.OnEnable();
        canFire = true;
    }

    private void FixedUpdate()
    {
        detected = Physics2D.OverlapCircle(transform.position, 5f, LayerMask.GetMask("Player"));

        if (detected != null && canFire)
        {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        canFire = false;
        fire = Managers.Resource.Instantiate("Fire");
        Managers.Sound.Play("flower_shoot");
        fire.transform.position = transform.position;
        fire.GetComponent<Fire>().isEnemy = true;
        fire.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(5,8)*Mathf.Sign(Managers.Game.GetPlayer().transform.position.x- transform.position.x), 
            Random.Range(12,18)), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        fire = Managers.Resource.Instantiate("Fire");
        fire.transform.position = transform.position;
        fire.GetComponent<Fire>().isEnemy = true;
        fire.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(5, 8) * Mathf.Sign(Managers.Game.GetPlayer().transform.position.x - transform.position.x),
            Random.Range(12, 18)), ForceMode2D.Impulse);
        yield return new WaitForSeconds(2f);
        canFire = true;
    }
    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }
}
