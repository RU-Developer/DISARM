using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 다트를 오브젝트 풀링하기위해 사용하는 클래스
 */
public class PoolableDart : Poolable
{
    /**
     * 다트 초기화 및 생성
     */
    public override Poolable Initialize()
    {
        gameObject.GetComponent<Dart>().Init();
        return this;
    }

    /**
     * 다트 소멸시 처리
     */
    public override Poolable Finalize()
    {
        return this;
    }
}
