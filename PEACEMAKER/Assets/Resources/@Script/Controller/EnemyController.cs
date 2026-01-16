using System;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Controller
{
    public class EnemyController : MonoBehaviour
    {
        public ECreatureStates CreatureState {get; set;}
        public DamageableObject Target {get; private set;}
        public CharacterController Controller {get; private set;}
        public Animator Animator {get; private set;}
        public Enemy Owner {get; private set;}
        public float Range { get; private set; }
        public bool ForceIdle;

        private void Awake()
        {
            CreatureState = ECreatureStates.Idle;
            Range = 2f;
            Controller = GetComponent<CharacterController>();
            Animator = GetComponent<Animator>();
            Owner = GetComponent<Enemy>();
            SetTarget();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 2f, Color.red);
            if (Target == null)
            {
                SetTarget();
                return;
            }
            
            //TODO 길찾기 알고리즘
            UpdateState();
            HandleState();
            Animator.SetFloat("velocity", Controller.velocity.magnitude * 3f);

        }

        private void UpdateState()
        {
#if UNITY_EDITOR
            if (ForceIdle)
            {
                CreatureState = ECreatureStates.Idle;
                return;
            }
#endif
            // 0. 이미 죽은 경우
            if (CreatureState == ECreatureStates.Dead)
                return;
            
            // 1. 스킬을 사용하고 있는경우
            if (CreatureState == ECreatureStates.Skill)
                return;
            
            // 2. Target이 없는 경우
            if (!Target)
            {
                SetTarget();
                CreatureState = ECreatureStates.Idle;
                return;
            }
            
            float distance = CalcDistance();
            // 3. Target이 Range안에 들어온 경우
            if (distance <= Range)
            {
                CreatureState = ECreatureStates.Skill;
            }
            // 4. Target이 Range를 벗어난 경우
            else
            {
                CreatureState = ECreatureStates.Moving;
            }
            
        }

        private void HandleState()
        {
            switch (CreatureState)
            {
                case ECreatureStates.Idle:
                    OnIdle();
                    break;
                case ECreatureStates.Moving:
                    OnMoving();
                    break;
                case ECreatureStates.Skill:
                    OnSkill();
                    break;
                case ECreatureStates.Dead:
                    break;
            }
        }

        private void OnIdle()
        {
            
        }

        private void OnMoving()
        {
            var dirVec =  Target.transform.position - transform.position;
            dirVec.y = 0;
            dirVec.Normalize();
            Controller.Move(dirVec * (3f * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(dirVec);
            
        }

        private void OnSkill()
        {
            Animator.Play("SkillA");
        }

        public void OnDead()
        {
            CreatureState =  ECreatureStates.Dead;
            //TODO 경험치 드랍.
        }

        private float CalcDistance()
        {
            var distance = Vector3.Distance(transform.position, Target.transform.position);
            return distance;
        }
        
        private void SetTarget()
        {
            Target = HeadManager.Game.MainPlayer;
        }

        public void ChangeState2Idle()
        {
            CreatureState = ECreatureStates.Idle;
        }

        public void Attack()
        {
            var dist = CalcDistance();
            //0. 범위에서 벗어낫으면
            if (dist > Range) return;

            //1. 여전히 범위 안에 있으면.
            Target.OnDamage(50, Owner);
        }
    }
}