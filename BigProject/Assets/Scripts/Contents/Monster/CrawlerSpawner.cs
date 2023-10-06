using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerSpawner : MonoBehaviour
{
    public Transform[] movePos;
    public bool wantSpawn;
    Collider2D detected;
    GameObject crawler;

    private void FixedUpdate()
    {
        if (!wantSpawn) return;
        detected = Physics2D.OverlapCircle(transform.position, 10f, LayerMask.GetMask("Player"));
        if (detected != null)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        wantSpawn = false;
        crawler = Managers.Resource.Instantiate("Crawler");
        crawler.GetComponent<Crawler>().movePos = movePos;
        crawler.GetComponent<Crawler>().moveNum = 0;
        yield return new WaitForSeconds(0.1f);
        crawler = Managers.Resource.Instantiate("Crawler");
        crawler.GetComponent<Crawler>().movePos = movePos;
        crawler.GetComponent<Crawler>().moveNum = 0;
        yield return new WaitForSeconds(0.1f);
        crawler = Managers.Resource.Instantiate("Crawler");
        crawler.GetComponent<Crawler>().movePos = movePos;
        crawler.GetComponent<Crawler>().moveNum = 0;
        yield return new WaitForSeconds(3f);
        wantSpawn = true;
    }
}
