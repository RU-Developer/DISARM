using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Scene들이 기본적으로 상속받는 클래스입니다.
 */
public abstract class BaseScene : MonoBehaviour
{
    /**
     * 현재 Scene 타입
     */
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    void Awake()
    {
        Init();
    }

    /**
     * Scene이 전환됬을 때 실행하는 부분
     * EventSystem 객체가 없으면 생성해준다.
     */
    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    /**
     * 장면 전환을 할 경우 여러 리소스들을 정리하는 부분이다.
     */
    public abstract void Clear();
}
