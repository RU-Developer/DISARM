using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour
{
    private const float moveSpeed = 15f;
    private bool fix;
    private Rigidbody2D rigid;
    private PlatformEffector2D pe;
    public PlayerStatus status;

    public static int dartLimit = 3;
    public static volatile int dartNum = 0;

    public void Init()
    {
        Start();
        rigid.velocity = Vector2.zero;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        pe = GetComponent<PlatformEffector2D>();
        pe.colliderMask = ~(1 << LayerMask.GetMask("Player"));
        pe.surfaceArc = 0;
    }

    private void Start()
    {
        fix = false;
        rigid = GetComponent<Rigidbody2D>();
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
            pe.surfaceArc = 90;
            pe.colliderMask = -1;
            pe.rotationalOffset = -transform.rotation.z*100;
            rigid.velocity = Vector2.zero;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void FixedUpdate()
    {
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
