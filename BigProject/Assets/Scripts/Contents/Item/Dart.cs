using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour
{
    private const float moveSpeed = 15f;
    private float time;
    private bool fix;
    private Rigidbody2D rigid;
    private BoxCollider2D[] colliders;
    private PlatformEffector2D pe;
    [HideInInspector]public PlayerStatus status;

    public static int dartLimit = 3;
    public static volatile int dartNum = 0;

    public void Init()
    {
        Start();
        rigid.velocity = Vector2.zero;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        time = 0.02f;
    }

    private void Start()
    {
        fix = false;
        rigid = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
        pe = GetComponent<PlatformEffector2D>();
        pe.surfaceArc = 0;
        pe.colliderMask &= ~(1 << LayerMask.NameToLayer("Player"));
        Managers.Resource.Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fix) return;
        if (collision.GetComponent<MonsterStatus>() != null)
        {
            collision.GetComponent<MonsterStatus>().OnDamaged(status, Vector2.zero, 10);
            Managers.Resource.Destroy(this.gameObject);
            Dart.dartNum--;
            Debug.Log($"dartNum = {Dart.dartNum}");
        }

        if (collision.tag == "Platform")
        {
            Managers.Sound.Play("dart_collision");
            fix = true;
            pe.colliderMask = ~0;
            pe.rotationalOffset = -rigid.rotation;
            pe.surfaceArc = 45;
            rigid.velocity = Vector2.zero;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void FixedUpdate()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;
            }
        }

        if (fix)
        {
            Managers.Resource.Destroy(this.gameObject, 3f, () => Dart.dartNum--);
            fix = false;
        }
        else
        {
            rigid.velocity = transform.up * moveSpeed; 
        }
    }
}
