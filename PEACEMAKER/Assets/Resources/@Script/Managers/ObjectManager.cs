using System;
using System.Collections.Generic;
using Resources.Script.Creatures;
using Resources.Script.InteractiveObject;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Managers
{
    public class ObjectManager
    {
        public HashSet<Enemy> Enemies { get; private set; } = new();
        public HashSet<ExpGem> ExpGems { get; private set; } = new();
        public HashSet<Player> Players { get; private set; } = new();

        
        public Transform GetRootTransform(string name)
        {
            GameObject root = GameObject.Find(name);
            if (root == null)
                root = new GameObject { name = name };

            return root.transform;
        }
        
        public Transform PlayersRoot { get { return GetRootTransform("@Players"); } }
        public Transform EnemiesRoot { get { return GetRootTransform("@Enemies"); } }
        public Transform ExpGemsRoot { get { return GetRootTransform("@ExpGems"); } }

        
        public T Spawn<T>(Vector3 pos) where T : BaseObject
        {
            string prefabName = typeof(T).Name;

            return Spawn<T>(prefabName, pos);
        }
        
        public T Spawn<T>(string objectName, Vector3 pos) where T : BaseObject
        {
            GameObject go = HeadManager.Resource.Instantiate(objectName);
            if (go == null)
            {
                Debug.LogError($"there's err while Instantiating: {objectName}");
                return go.GetComponent<T>();
            }
            
            go.name = objectName;
            go.transform.position = pos;
            BaseObject obj = go.GetComponent<BaseObject>();

            switch (obj.ObjectType)
            {
                case EObjectType.Enemy:
                    Enemies.Add(go.GetComponent<Enemy>());
                    break;
                case EObjectType.ExpGem:
                    var gem =  obj.GetComponent<ExpGem>();
                    ExpGems.Add(gem);
                    gem.Init(pos, objectName[^3..]);
                    break;
                case EObjectType.Player:
                    Players.Add(go.GetComponent<Player>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return obj as T;
        }

        public void Despawn<T>(T obj) where T : BaseObject
        {
            EObjectType objectType = obj.ObjectType;
            switch (objectType)
            {
                case EObjectType.Enemy:
                    Enemies.Remove(obj.GetComponent<Enemy>());
                    break;
                case EObjectType.ExpGem:
                    ExpGems.Remove(obj.GetComponent<ExpGem>());
                    break;
                case EObjectType.Player:
                    Players.Remove(obj.GetComponent<Player>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            HeadManager.Resource.Destroy(obj.gameObject);
        }
    }
    
}