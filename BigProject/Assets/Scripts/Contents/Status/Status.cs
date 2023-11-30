using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 공격할 수 있고, 공격 받을 수 있는 Damageable 한 GameObject들이 가질 컴포넌트
 * 죽었을 시와, 죽였을 시의 행동을 재정의 해서 사용할 수 있다.
 * 죽었을 때 아이템 드랍이나 죽였을 때 스텟 증가 등의 이벤트를 발생시킬 때 사용하자
 */ 
public abstract class Status : Despawnable
{
    private int _hp;
    private int _maxHp;
    private int _attack;
    private float r, g, b;
    private SpriteRenderer[] sprites;

    public int Hp { get { return _hp; } protected set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } protected set { _maxHp = value; } }
    public int Attack { get { return _attack; } protected set { _attack = value; } }

    private Rigidbody2D rigid;

    private void Start()
    {
        Init();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            r = sprites[i].color.r; g = sprites[i].color.g; b = sprites[i].color.b;
        }
    }

    protected virtual void Init()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    /**
     * damage를 직접 입력하면 damage를 감소시키고,
     * 기본 공격일 경우 attacker의 Attack을 감소시킨다.
     */
    public virtual void OnDamaged(Status attacker, Vector2 knockback = new Vector2(), int damage = -1)
    {
        if (damage < 0)
            damage = attacker.Attack;
        Managers.Sound.Play("get_hit");
        _hp -= damage;

        BaseController controller = null;
        if ((controller = gameObject.GetComponent<BaseController>()) != null &&
            (controller.WorldObjectType == Define.WorldObject.Player || controller.WorldObjectType == Define.WorldObject.Monster))
            controller.OnHit();

        StartCoroutine(Hit());
        if (knockback != new Vector2())
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(knockback + new Vector2(0, 1f), ForceMode2D.Impulse);
        }

        if (_hp <= 0)
        {
            _hp = 0;
            OnDead(attacker);
            attacker.OnKill(this);
        }
    }

    protected IEnumerator Hit()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = new Color(255, 0, 0);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = new Color(r, g, b);
        }
    }

    /**
     * 이 스텟의 주인이 죽었을 때
     */
    public virtual void OnDead(Status attacker)
    {
        Debug.Log($"{name}이(가) {attacker.name}에게 죽었습니다.");
    }

    /**
     * 스텟의 주인이 대상을 죽였을 때
     */
    public virtual void OnKill(Status deadTarget)
    {
        Debug.Log($"{name}이(가) {deadTarget.name}을(를) 죽었습니다.");
    }
}
