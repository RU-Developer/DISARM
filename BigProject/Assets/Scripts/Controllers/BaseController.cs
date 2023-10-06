using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * GameManager가 관리하는 자원들은 이 클래스를 상속받게 됩니다.
 */
public class BaseController : MonoBehaviour
{
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    private void Awake()
    {
        Init();
    }

    public virtual void Init()
    {

    }

    public virtual void OnSpawn()
    {

    }

    public virtual void OnDeSpawn()
    {
        Despawnable[] arr = gameObject.GetComponents<Despawnable>();
        for (int i = 0; i < arr.Length; i++)
            arr[i].Despawn();

        List<Despawnable> despawnables = gameObject.FindAllChild<Despawnable>();
        if (despawnables != null)
            foreach (Despawnable item in despawnables)
                item.Despawn();
    }
}
