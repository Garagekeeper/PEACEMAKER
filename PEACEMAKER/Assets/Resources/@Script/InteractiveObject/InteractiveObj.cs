using UnityEngine;

namespace Resources.Script.InteractiveObject
{
    public class InteractiveObj : MonoBehaviour, IPoolable
    {
        public virtual void OnSpawn()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnDespawn()
        {
            gameObject.SetActive(false);
        }
        
    }
}