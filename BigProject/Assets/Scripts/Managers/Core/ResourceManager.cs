using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Prefab 등의 리소스를 게임에 생성하고 삭제하는 등의 작업을 관리하는 클래스입니다.
 */
public class ResourceManager
{
    /**
     * Resource를 경로에서 찾아 로딩
     */
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        //Pool에 있으면 사용
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    /**
     * Resource를 경로에서 찾아 GameObject로 로딩한 후, 생성
     * 이미 풀링중인 경우 풀에서 꺼내서 사용
     */
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        //Pooling 된 GameObject가 있다면 Pool을 사용
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = UnityEngine.Object.Instantiate(original, parent);
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);

        return go;
    }

    /**
     * 생성된 GameObject를 소멸
     * 오브젝트 풀링중일 경우 Pool에 반환
     * 풀링하는 경우 추가로 제거시 이벤트 action 사용 가능
     */
    public void Destroy(GameObject go, float time = 0.0f, Action action = null)
    {
        if (go.IsValid() == false)
            return;

        Poolable poolable = go.GetComponent<Poolable>();

        if (poolable == null)
        {
            UnityEngine.Object.Destroy(go, time);
        }
        else
        {
            if (time != 0.0f)
            {
                //시간이 지정된 경우 시간초 후에 다시 호출
                poolable.StartCoroutine(Managers.Resource.DelayPooling(poolable, time, action));
                return;
            }
            //만약 풀링이 필요할 경우 풀에 반환 
            if (poolable.IsUsing)
                Managers.Pool.Push(poolable);
        }
    }

    /**
     * time 초 후에 풀링
     */ 
    private IEnumerator DelayPooling(Poolable poolable, float time, Action action = null)
    {
        yield return new WaitForSeconds(time);
        if (poolable.IsUsing)
            Managers.Pool.Push(poolable);

        if (action != null)
            action.Invoke();
    }
}
