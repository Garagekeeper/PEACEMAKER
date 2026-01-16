using Resources.Script.Creatures;
using UnityEngine;

namespace Resources.Script.InteractiveObject
{
    public class InteractiveObj : BaseObject
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