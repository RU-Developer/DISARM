using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 유용하게 활용 가능한 여러 함수를 모아둔 곳입니다.
 */
public class Util
{
    /**
     * GameObject로부터 Component를 얻어내는 함수입니다.
     * 해당 Component가 없으면 해당 Component를 추가한 후 반환합니다.
     */
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    /**
     * GameObject의 밑에서 name으로 Child GameObject를 찾습니다.
     * recursive 파라미터를 통해 자식을 재귀적으로 탐색할지 정할 수 있습니다.
     */
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    /**
     * GameObject의 밑에서 name으로 Component를 찾습니다.
     * recursive 파라미터를 통해 자식을 재귀적으로 탐색할지 정할 수 있습니다.
     */
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static List<Transform> FindAllChild(GameObject go, string name = null, bool recursive = false)
    {
        List<Transform> transforms = FindAllChild<Transform>(go, name, recursive);
        if (transforms == null)
            return null;

        return transforms;
    }

    public static List<T> FindAllChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        List<T> components = new List<T>();

        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        components.Add(component);
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    components.Add(component);
            }
        }

        return components;
    }
}
