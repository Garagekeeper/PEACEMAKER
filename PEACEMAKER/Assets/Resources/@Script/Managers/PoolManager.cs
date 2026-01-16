using System.Collections.Generic;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Pool;

// 유니티에서 제공하는 Pool 사용
internal class Pool
{
    private GameObject _prefab;
    public IObjectPool<GameObject> _pool;
    
    private Transform _rootTransform;

    public Transform RootTransform
    {
        get
        {
            if (_rootTransform == null)
            {
                GameObject go = new GameObject($"@{_prefab.name}Pool");
                _rootTransform = go.transform;
            }

            return _rootTransform;
        }

    }

    public Pool(ObjectPreset preset)
    {
        _prefab = preset.prefab;
        // Unity에서 제공하는 Pool
        // createFunc, OnGet, OnRelease, OnDestroy, 중복 반환 여부, 초기사이즈 , 최대사이즈
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy, true, preset.initialSize);
    }

    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy, true, 10);
    }

    private GameObject OnCreate()
    {
        GameObject go = Object.Instantiate(_prefab, RootTransform, true);
        go.name = _prefab.name;
        return go;
    }

    private void OnGet(GameObject go)
    {
        go.SetActive(true);
    }
    
    private void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
    
    private void OnDestroy(GameObject go)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        Object.Destroy(go);
    }

    // 풀에 push하는건 사용이 끝나서 반환 하는것
    public void Push(GameObject go)
    {
        if (go.activeSelf)
            _pool.Release(go);
    }

    // 사용하기 위해서 Pool에서 가져옴
    public GameObject Pop()
    {
        return _pool.Get();
    }
}

public class PoolManager
{
    private Dictionary<string, Pool> _pools =  new ();

    // 풀에 반환
    public bool Push(GameObject go)
    {
        // 해당 오브젝트를 위한 풀이 없으면 실패
        if (!_pools.ContainsKey(go.name))
            return false;

        _pools[go.name].Push(go);
        return true;
    }

    public GameObject Pop(ObjectPreset objectPreset)
    {
        if (!_pools.ContainsKey(objectPreset.prefab.name))
            CreatePool(objectPreset);
        
        return  _pools[objectPreset.prefab.name].Pop();
    }
    
    public GameObject Pop(GameObject prefab)
    {
        if (!_pools.ContainsKey(prefab.name))
            CreatePool(prefab);
        
        return  _pools[prefab.name].Pop();
    }

    private void CreatePool(ObjectPreset objectPreset)
    {
        Pool pool = new Pool(objectPreset);
        _pools.Add(objectPreset.prefab.name, pool);
    }
    
    private void CreatePool(GameObject prefab)
    {
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }
    
}
