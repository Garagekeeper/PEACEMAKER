using UnityEngine;

namespace Resources.Script.Creatures
{
    /// <summary>
    /// Ragdoll 효과를 활성/비활성해주는 함수
    /// </summary>
    public class RagdollEffect : MonoBehaviour
    {
        public Animator animator;
        public bool isEnabled;

        private Rigidbody[] _rigidbodies;
        
        protected virtual void Start()
        {
            // 애니메이터와, 자식들의 강체를 모두 들고 있는다.
            if (animator == null)
                animator = transform.FindSelfChildParent<Animator>();
            
            _rigidbodies = GetComponentsInChildren<Rigidbody>();

            if (isEnabled)
                Enable();
            else
                Disable();
        }
        
        protected virtual void Update()
        {
            foreach(Rigidbody rb in _rigidbodies) rb.isKinematic = !isEnabled;
        }

        // 애니메이터와 isKinematic을 끈다. (애니메이션이 아니라 유니티 물리를 적용)
        public virtual void Enable()
        {
            isEnabled = true;
            animator.enabled = false;
        }

        // 애니메이터와 isKinematic을 켠다. (유니티 물리가 아니라 애니메이션을 적용)
        public virtual void Disable()
        { 
            isEnabled = false;
            animator.enabled = true;
        }
    }
}