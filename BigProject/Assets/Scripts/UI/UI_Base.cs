using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * UI의 기반이 되는 클래스입니다.
 */
public abstract class UI_Base : MonoBehaviour
{
    //UI가 가진 요소들을 UI 클래스를 통해 Dictionary에 바인딩합니다.
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    private void Start()
    {
        Init();
    }

    /**
     * Start가 호출될 때 Init()함수를 호출해주면서 초기화 시키는 역할입니다.
     */
    public abstract void Init();

    /**
     * Enum타입으로 정의된 UI요소들을 Dictionary에 바인딩 해주는 함수입니다.
     */
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    /**
     * 바인딩 된 요소를 꺼내오는 함수입니다.
     */
    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[index] as T;
    }

    protected GameObject GetObject(int index)
    {
        return Get<GameObject>(index);
    }

    protected Text GetText(int index)
    {
        return Get<Text>(index);
    }

    protected Button GetButton(int index)
    {
        return Get<Button>(index);
    }

    protected Image GetImage(int index)
    {
        return Get<Image>(index);
    }

    /**
     * UI Event를 추가해주는 함수입니다.
     * UI GameObject를 받아서 이벤트를 등록합니다.
     */
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.AddOnClickHandler(action);
                break;
            case Define.UIEvent.Drag:
                evt.AddOnDragHandler(action);
                break;
        }
    }

    public virtual void Clear()
    {
    }
}
