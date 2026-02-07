using System;
using Resources.Script.Audio;
using Resources.Script.Creatures;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Serialization;
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
        
        [SerializeField] private float gravity = Physics.gravity.y;
        private float _verticalVelocity;
        
        [SerializeField] private AudioPreset audioPreset; // 소리를 재생할 컴포넌트
        [SerializeField] private float soundInterval = 0.5f; // 소리 재생 간격 (초)

        private float soundTimer; // 시간을 잴 타이머
        
        
        private void Awake()
        {
            CreatureState = ECreatureStates.Idle;
            Range = 2f;
            Controller = GetComponent<CharacterController>();
            Animator = GetComponent<Animator>();
            Owner = GetComponent<Enemy>();
            SetTarget();
        }
        
        /// <summary>
        /// 소환시 호출되는 초기화 함수
        /// </summary>
        /// <param name="spawnPos">소환 위치를 조절할 파라미터</param>
        public void InitOnSpawn(Vector3 spawnPos)
        {
            if (Controller == null) Controller = GetComponent<CharacterController>();
            Controller.enabled = false;
            
            _verticalVelocity = 0f;
            soundTimer = 0f;
            CreatureState = ECreatureStates.Idle;
            
            transform.position = spawnPos;
            
            Controller.enabled = true;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 2f, Color.red);
            
            ApplyGravity();
            
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

        private void ApplyGravity()
        {
            if (CreatureState == ECreatureStates.Dead) return;
            if (Controller.isGrounded)
            {
                // 땅에 붙어있게 살짝 눌러줌
                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            Controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
        }

        private void UpdateState()
        {
#if UNITY_EDITOR
            if (ForceIdle && CreatureState != ECreatureStates.Dead)
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
            Controller.Move(dirVec * (Owner.Speed * Owner.SpeedMultiplier * Time.deltaTime));
            transform.rotation = Quaternion.LookRotation(dirVec);
            
            // --- 소리 재생 로직 추가 ---
            soundTimer += Time.deltaTime; // 흐른 시간을 더함

            if (soundTimer >= soundInterval)
            {
                PlayMoveSound();
                soundTimer = 0f; // 타이머 초기화
            }
        }
        
        private void PlayMoveSound()
        {
            HeadManager.Audio.PlayWithPreset(audioPreset, transform);
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
            Target.OnDamage(50, Owner, Target.transform.position);
        }
    }
}