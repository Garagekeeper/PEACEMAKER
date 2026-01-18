using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Managers
{
    [CreateAssetMenu(fileName = "New PoolData", menuName = "PEACEMAKER/PoolData")]
    public class ObjectPreset : ScriptableObject
    {
        
        public GameObject prefab;
        public EObjectID id;
        
        public bool poolable = false; 
        public int initialSize = 30;
    }
}
