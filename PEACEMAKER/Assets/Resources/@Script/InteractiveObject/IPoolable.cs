using UnityEngine;

namespace Resources.Script.InteractiveObject
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}