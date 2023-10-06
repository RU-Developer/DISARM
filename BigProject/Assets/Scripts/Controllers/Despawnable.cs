using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * BaseController에서 자동으로 리소스를 해제해주기 위해 붙이는 컴포넌트입니다.
 * 이를 상속받은 컴포넌트들은 BaseController 컴포넌트가 붙어있고,
 * GameManager를 통해 Despawn되었을 경우, 자동으로 리소스를 해제해 주게 됩니다.
 */
public abstract class Despawnable : MonoBehaviour
{
    public abstract void Despawn();
}
