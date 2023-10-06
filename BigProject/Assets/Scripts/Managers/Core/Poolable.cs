using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 이 컴포넌트가 붙어있는 Prefab은 ResourceManager를 사용하여 생성 삭제시
 * PoolManager에 의해 오브젝트 풀링 기법이 적용됩니다.
 */
public class Poolable : MonoBehaviour
{
    public bool IsUsing; //현재 사용 가능한 상태인지 확인하는 필드

    /**
     * 오브젝트 풀링에서 생성자 및 초기화 역할을 대신하는 부분
     */ 
    public virtual Poolable Initialize()
    {
        return this;
    }

    /**
     * 오브젝트 풀링에서 소멸자 역할을 대신하는 부분
     */
    public virtual Poolable Finalize()
    {
        return this;
    }
}
