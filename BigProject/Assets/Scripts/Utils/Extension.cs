using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * 다른 클래스의 함수를 확장하는 클래스
 */
public static class Extension
{
    /**
     * UI_Base 클래스의 BindEvent 함수의 매개변수로 들어가는 GameObject를 통해 호출할 수 있도록 하는 함수
     * ex) gameObject.BindEvent((PointerEventData) => { Debug.Log("hello"); }, Define.UIEvent.Click);
     */
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }
 
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    /**
     * 게임 오브젝트가 Destroy 되었을 때 유니티에서 자체적으로 참조하고 있던 객체를
     * "null" 이라고 넣어주는데 operator overloading을 통해 null과 GameObject를 비교할 수 있게 해준다.
     * Destroy되면 null이라고 비교할 수 있는데,
     * 자체적으로 오브젝트 풀링 기법을 활용중이어서 null과 비교하는 것 뿐만 아니라
     * 풀링 되어있지 않은지 직접 체크해봐야 한다.
     * Poolable컴포넌트를 가져와서 IsUsing을 체크하거나
     * GameObject의 activeSelf 를 확인하여 활성화되어있는지 확인하면 된다.
     */
    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public static GameObject FindChild(this GameObject go, string name = null, bool recursive = false)
    {
        return Util.FindChild(go, name, recursive);
    }

    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false)
        where T : UnityEngine.Object
    {
        return Util.FindChild<T>(go, name, recursive);
    }

    public static List<Transform> FindAllChild(this GameObject go, string name = null, bool recursive = false)
    {
        return Util.FindAllChild(go, name, recursive);
    }

    public static List<T> FindAllChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        return Util.FindAllChild<T>(go, name, recursive);
    }
}
