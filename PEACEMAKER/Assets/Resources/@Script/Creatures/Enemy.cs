using System;
using System.Collections;
using Resources.Script.Controller;
using Resources.Script.InteractiveObject;
using Resources.Script.Managers;
using Resources.Script.UI.Scene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static Resources.Script.Defines;

namespace Resources.Script.Creatures
{
    public class Enemy : DamageableObject, IMovable
    {
        public float Speed { get; set; } = 3f;
        public float SpeedMultiplier { get; set; } = 1f;
        
        public ERarity Rarity { get; set; }
        private EnemyController  _controller;
        public EObjectID GemID { get; set; }
        
        public Action<ERarity> OnEnemyKilled { get; set; }
        public event Action<DamageInfo> OnDamaged;
        
        protected RagdollEffect _ragdollEffect;
        public RagdollEffect RagdollEffect
        {
            get
            {
                if (_ragdollEffect == null) _ragdollEffect = GetComponent<RagdollEffect>();
                return _ragdollEffect;
            }
            set => _ragdollEffect = value;
        }
        
        protected override void Awake()
        {
            base.Awake();
            CreatureType = ECreatureType.Enemy;
            _controller = GetComponent<EnemyController>();
            Rarity = ERarity.Normal;
            GemID = EObjectID.ExpGemNormal + (int)Rarity;
            ObjectType = EObjectType.Enemy;
            RagdollEffect = GetComponent<RagdollEffect>();
        }

        public void InitOnSpawn()
        {
            Hp = maxHp;
            DisableRagRoll();
            _minimapMarkerRenderer.enabled = true;
            var SceneUI = (UIGameScene)HeadManager.UI.SceneUI;
            OnDamaged += SceneUI.ShowDamageText;
            OnDamaged += SceneUI.ShowHitmarker;
        }

        public override void OnDamage(float value, Creature attackBy, Vector3 hitPos, bool isCrit = false)
        {
            var info = new DamageInfo
            {
                amount = Hp - value <= 0 ? Hp : value,
                //TODO hitpoint에 따른 위치 조절
                hitPoint = transform.position + Vector3.up * 1.2f,
                isCrit = isCrit
            };
            OnDamaged?.Invoke(info);
            base.OnDamage(value, attackBy, hitPos, isCrit);
        }

        public override void OnDeath()
        {
            base.OnDeath();
            EnableRagRoll();
            _controller.OnDead();
            GetComponent<CharacterController>().enabled = false;
            
            // spawn EXP gem
            HeadManager.ObjManager.Spawn<ExpGem>(GemID, dropTransform.position);
            
            // Call Adding Score Event
            OnEnemyKilled?.Invoke(Rarity);
            
            // 연결되어있던 이벤트 언바인딩
            UIGameScene SceneUI = HeadManager.UI.SceneUI as UIGameScene;
            OnDamaged -= SceneUI.ShowDamageText;
            OnDamaged -= SceneUI.ShowHitmarker;
            
            StartCoroutine(WaitDelay(gameObject, 5));
        }
        
        public void Destroy(GameObject go)
        {
            HeadManager.ObjManager.Despawn(this);
        }

        private IEnumerator WaitDelay(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(go);
        }
        
        public void EnableRagRoll()
        {
            RagdollEffect.Enable();
        }
        
        public void DisableRagRoll()
        {
            RagdollEffect.Disable();
        }
    }
}