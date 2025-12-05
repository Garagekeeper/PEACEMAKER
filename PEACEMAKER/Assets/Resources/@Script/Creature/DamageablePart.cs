using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Resources.Script.Creature
{
    public class DamageablePart : MonoBehaviour, IDamageablePart
    {
        [SerializeField] private HumanBodyBones mBone;
        [SerializeField] private float mDamageMultiplier = 1;
        [SerializeField] private bool mIsCritical;
        private string name;

        public IDamageable ParentDamageable { get; private set; }
        public HumanBodyBones Bone => mBone;
        public string PartName => name;
        public float DamageMultiplier => mDamageMultiplier;
        public bool IsCriticalPart => mIsCritical;

        // Avatar가 할당되면 Skinned Rig 구조가 재배치된다
        // 리타게팅 후 실제 위치로 고정
        private void Awake()
        {
            // var animator = GetComponentInParent<Animator>();
            // var actualBone = animator.GetBoneTransform(Bone);
            // if (actualBone != transform)
            // {
            //     // 이 스크립트를 실제 본으로 재배치
            //     transform.SetParent(actualBone, false);
            // }
        }

        private void Start()
        {
            ParentDamageable = GetComponentInParent<IDamageable>();
            name = gameObject.name;
        }
    }
}