using UnityEngine;

namespace Resources.Script.Managers
{
    [CreateAssetMenu(fileName = "New PoolData", menuName = "PEACEMAKER/PoolData")]
    public class ObjectPreset : ScriptableObject
    {
        public GameObject prefab;

        public bool poolable = false; 
        public int initialSize = 30;
    }
}
