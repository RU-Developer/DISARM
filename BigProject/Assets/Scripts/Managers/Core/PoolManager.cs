using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ResourceManager를 사용하여 Prefab을 생성 삭제할 때,
 * 오브젝트 풀링 기법을 적용하는 클래스입니다.
 */
public class PoolManager
{
    /**
     * Prefab 하나에 대한 오브젝트 풀을 제어하는 클래스
     * Original(Prefab)을 가지고, 그 정보를 통해 GameObject 생성
     */ 
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        /**
         * Prefab이름_Root 라는 이름으로 GameObject를 생성하여
         * 그 밑에 풀링되는 오브젝트들을 넣어놓는다.
         */
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{Original.name}_Root";

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        /**
         * Prefab을 통해 GameObject를 생성하고, Poolable 컴포넌트를 붙여놓는다.
         */ 
        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        /**
         * 오브젝트 풀에 등록 및 반환
         */
        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        /**
         * 오브젝트 풀에서 꺼내기
         */
        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            //DontDestroyOnLoad 상태를 해제하기 위함
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;

            poolable.transform.parent = parent;
            poolable.gameObject.SetActive(true);
            poolable.IsUsing = true;

            return poolable;
        }
    }

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    /**
     * 오브젝트 풀에 반환하는 함수
     * 풀이 없는 오브젝트일 경우 그냥 삭제
     */
    public void Push(Poolable poolable)
    {
        string name = poolable.name;
        if (_pool.ContainsKey(name) == false)
        {
            Object.Destroy(poolable);
            return;
        }

        poolable.StopAllCoroutines();
        poolable.Finalize();

        _pool[name].Push(poolable);
    }

    /**
     * 오브젝트 풀에서 꺼내는 함수
     * 풀이 없거나, Poolable이 붙어있지 않아도, Poolable을 붙여준다.
     */ 
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pool[original.name].Pop(parent).Initialize();
    }

    /**
     * 등록된 오브젝트 풀이 없을 때, 오브젝트 풀을 생성하는 함수
     */
    void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    /**
     * 해당 오브젝트풀이 등록되어있으면 Original(Prefab)정보를 돌려준다.
     */
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
