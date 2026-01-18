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
        public Transform SoundRoot { get { return GetRootTransform("@Sounds"); } }
        
        public T Spawn<T>(EObjectID id, Vector3 pos) where T : BaseObject
        {
            GameObject go = HeadManager.Resource.Instantiate(id);
            if (go == null)
            {
                Debug.LogError($"there's err while Instantiating: {id}");
                return go.GetComponent<T>();
            }
            
            //go.name = objectName;
            go.transform.position = pos;
            BaseObject obj = go.GetComponent<BaseObject>();

            switch (obj.ObjectType)
            {
                case EObjectType.Enemy:
                    go.transform.SetParent(EnemiesRoot);
                    Enemies.Add(go.GetComponent<Enemy>());
                    break;
                case EObjectType.ExpGem:
                    var gem =  obj.GetComponent<ExpGem>();
                    go.transform.SetParent(ExpGemsRoot);
                    ExpGems.Add(gem);
                    gem.Init(pos, id);
                    break;
                case EObjectType.Player:
                    go.transform.SetParent(PlayersRoot);
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