using System;
using UnityEngine;

public class MonsterController : BaseController
{
    public bool dead { get; private set; }
    public event Action onDeath;

    protected MonsterStatus status;

    public override void Init()
    {
        base.Init();
        status = gameObject.GetComponent<MonsterStatus>();
        WorldObjectType = Define.WorldObject.Monster;
    }

    protected virtual void OnEnable()
    {
        dead = false;
    }

    public virtual void Die()
    {
        if (onDeath != null)
        {
            onDeath();
        }
        dead = true;
    }
}
