using System;
using System.Collections.Generic;
using Resources.Script.Creatures;
using Resources.Script.InteractiveObject;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Managers
{
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] private PoolDefinition[] poolDataClassArray;
        /// <summary>
        /// 타입에 따른 풀의 정보를 들고있는 딕셔너리
        /// </summary>
        private Dictionary<Type, PoolDefinition> _poolDefDict = new();
        /// <summary>
        /// 실제 풀을 들고있는 딕셔너리
        /// </summary>
        private Dictionary<Type, Queue<IPoolable>> _poolDict = new();
        private GameObject _root;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _root = GameObject.Find("@ObjectPool");
            if (_root == null)
            {
                _root =  new GameObject("@ObjectPool"); 
                DontDestroyOnLoad(_root);
            }

            InitPool();
        }

        private void InitPool()
        {
            foreach (var poolData in poolDataClassArray)
            {
                // 프리팹이 아예 없는 경우 필터
                if (!poolData.prefab)
                {
                    Debug.LogError("[Object Manager]There is no pool bass prefab");
                    continue;
                }
                
                var poolable = poolData.prefab.GetComponent<IPoolable>();
                
                // 프리팹은 있는데 풀링 오브젝트가 아닌 경우 필터
                if (poolable == null)
                {
                    Debug.LogError($"{poolData.prefab.name} has no IPoolable");
                    continue;
                }
                
                // 부모 오브젝트가 null인 경우 부모를 만들어준다.
                if (!poolData.parent)
                {
                    string name = poolData.prefab.name + "GemPool";
                    var parent = new GameObject(name); 
                    parent.transform.SetParent(_root.transform);
                    poolData.parent = parent.transform;
                }
                
                // Queue 초기화
                // 런타입에 결정된 타입을 들고옴
                Type poolType = poolable.GetType();
                
                if (_poolDict.ContainsKey(poolType))
                {
                    Debug.LogWarning($"{poolType.Name} pool already exists.");
                    continue;
                }
                
                // DICT에 추가
                _poolDict[poolType] = new Queue<IPoolable>();
                _poolDefDict[poolType] = poolData;

                // 초기 사이즈만큼 풀에 추가
                for (var i = 0; i < poolData.initialSize; i++)
                {
                    AddNewSource2Pool(poolType);
                }
            }
        }
        
        /// <summary>
        /// 타입에 맞는 풀에 오브젝트를 추가하는 함수
        /// </summary>
        /// <param name="type"></param>
        private void AddNewSource2Pool(Type type)
        {
            // 0. 해당 타입에 관한 풀링 데이터가 없으면 종료
            if (!_poolDefDict.TryGetValue(type, out var poolData))
            {
                Debug.LogError($"No PoolDefinition for {type.Name}");
                return;
            }

            // 1. 데이터를 기반으로 오브젝트 인스턴스화
            var instance = Instantiate(poolData.prefab, poolData.parent);
            var poolable = instance.GetComponent<IPoolable>();

            // 2. 풀에 들어갈때는 꺼진 상태로 들어가게
            poolable.OnDespawn();
            
            // 3. dict에서 해당하는 타입의 풀링 큐에 추가
            _poolDict[type].Enqueue(poolable);
        }
        
        /// <summary>
        /// 풀에서 오브젝트를 가져오는 함수
        /// 호출 시 타입을 명시할 것
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSourceFromPool<T>() where T : MonoBehaviour, IPoolable
        {
            var type = typeof(T);
            // 0. 해당 타입의 풀이 없으면 종료
            if (!_poolDict.TryGetValue(type, out var queue))
            {
                Debug.LogError($"[ObjectManager] GetSourceFromPool: There is no Queue with {type.Name}");
                return null;
            }

            // 1. 큐가 비었으면 새로만들어서 삽입.
            // TODO (몇개 삽입할지 고민 해보기)
            if (queue.Count == 0)
            {
                Debug.LogWarning("[ObjectManager] NotEnough pool");
                AddNewSource2Pool(type);
            }
            
            // 2. 큐에서 아이템을 꺼내서 반환
            var item = (T)queue.Dequeue();
            item.OnSpawn();
            return item;
        }

        /// <summary>
        /// 사용이 끝난 오브젝트를 풀에 반환
        /// </summary>
        /// <param name="poolable"></param>
        public void ReturnSourceToPool(IPoolable poolable)
        {
            // 0. 런타임 타입을 통해서 해당 타입의 풀이 있는지 검색
            Type type = poolable.GetType();
            if (!_poolDict.TryGetValue(type, out var queue))
            {
                Debug.LogError($"[ObjectManager] ReturnSourceToPool: There is no Queue with {type.Name}");
                return;
            }
            
            // 1. 풀에 넣을때는 꺼진 상태로
            poolable.OnDespawn();
            _poolDict[type].Enqueue(poolable);
        }

        public void SpawnGem(ERarity rarity, Creature spawnedFrom)
        {
            ExpGem spawnedGem = null;

            // 등급에 따라 다른 클래스 타입을 제네릭으로 넘김
            switch (rarity)
            {
                case ERarity.Normal:
                    spawnedGem = HeadManager.ObjManager.GetSourceFromPool<NormalGem>();
                    break;
                case ERarity.Rare:
                    spawnedGem = HeadManager.ObjManager.GetSourceFromPool<RareGem>();
                    break;
                case ERarity.Epic:
                    spawnedGem = HeadManager.ObjManager.GetSourceFromPool<EpicGem>();
                    break;
            }

            if (!spawnedGem)
            {
                Debug.LogError("[Enemy] SpawnGem : spawned gem is null");
                return;
            }

            spawnedGem.Init(spawnedFrom.dropTransform.position, rarity); // 필요한 초기화
            
        }
    }
    
    [System.Serializable]
    public class PoolDefinition
    {
        public GameObject prefab;
        public int initialSize = 30;
        public Transform parent;
    }
}