using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStatus : NonDamageableEnvStatus
{
    protected override void Init()
    {
        base.Init();
        Data.MonsterStatus turret = Managers.Data.MonsterStatusDict["Turret"];
        MaxHp = turret.maxHp;
        Hp = turret.maxHp;
        Attack = turret.attack;
    }

    public override void OnDamaged(Status attacker, Vector2 knockback = default, int damage = -1)
    {
        if (damage < 0)
            damage = attacker.Attack;

        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attacker);
            attacker.OnKill(this);
        }
    }

    public override void OnDead(Status attacker)
    {
        Managers.Game.Despawn(gameObject);
    }
}
