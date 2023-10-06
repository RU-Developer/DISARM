using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonDamageableEnvStatus : Status
{
    protected override void Init()
    {
        base.Init();
        Data.NonDamageableEnvStatus status = Managers.Data.NonDamageableEnvStatusDict[name];
        Attack = status.attack;
    }

    public override void OnDamaged(Status attacker, Vector2 knockback = new Vector2(), int damage = -1)
    {
        Debug.Log("Can't attack Environment");
    }

    public override void Despawn()
    {
    }
}
