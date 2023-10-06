using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatus : Status
{
    public override void Despawn()
    {

    }

    protected override void Init()
    {
        base.Init();
        Data.MonsterStatus status = Managers.Data.MonsterStatusDict[name];
        Hp = status.maxHp;
        MaxHp = status.maxHp;
        Attack = status.attack;
    }

    public override void OnDead(Status attacker)
    {
        Managers.Game.Despawn(gameObject);
    }
}
