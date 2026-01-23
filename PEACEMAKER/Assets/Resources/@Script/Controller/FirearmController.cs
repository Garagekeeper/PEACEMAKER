using System;
using System.Collections.Generic;
using Resource.Script.Controller;
using Resources.Script.Ammo;
using Resources.Script.Creatures;
using Resources.Script.Firearm;
using Resources.Script.Managers;
using Resources.Script.UI;
using Resources.Script.UI.Scene;
using UnityEngine;
using UnityEngine.Serialization;
using static Resources.Script.Defines;
using FirearmPreset = Resources.Script.Firearm.FirearmPreset;

namespace Resources.Script.Controller
{
    public class FirearmController : MonoBehaviour
    {
        public FirearmPreset preset;
        
        [Tooltip("The Transform from which the shots are fired.")]
        public Transform muzzle;

        public event Action<int, int, int> OnAmmoChanged;
        public event Action<string> OnFirearmNameChanged;
     
        /// <summary>
        /// The number of shots fired in the current session.
        /// </summary>
        public int ShotsFired { get; protected set; }

        /// <summary>
        /// Whether the firearm is currently firing.
        /// </summary>
        public bool IsFiring { get; set; }
        
        /// <summary>
        /// 화기가 사격을 할 준비가 되어 있는지
        /// Whether the firearm is ready to fire.
        /// </summary>
        public bool ReadyToFire { get; protected set; }
        
        /// <summary>
        /// UI조작등 외부요인으로 사격을 금지해야할 때.
        /// A flag that determines whether the firing action is prevented. 
        /// This is useful for disabling the ability to fire, such as when the player is interacting with an inventory UI or performing other non-combat actions.
        /// </summary>
        public bool FirePrevented { get; set; }
        
        public EFirearmStates FirearmState { get; set; }
        public EFirearmAimStates FirearmAimState { get; set; }
        public EVector3Direction DecalDirection { get; set; }
        
        public PlayerController OwnerController { get; set; }
        public IDamageable Owner;
        public List<ProjectileController> Projectiles { get; set; }
        
        /*-------------------------
         *           Ammo
        -------------------------*/
        
        /// <summary>
        /// 현재 화기에 필요한 탄약 타입 클래스
        /// </summary>
        public AmmoType requiredAmmoType;
        /// <summary>
        /// 실제 가지고 있는 탄약 수
        /// num of bullet player has;
        /// </summary>
        public AmmoItem ammoItemInInventory;

        private int _ammoInMagazine;
        public int AmmoInMagazine
        {
            get => _ammoInMagazine;
            set
            {
                _ammoInMagazine = value;
                OnAmmoChanged?.Invoke(_ammoInMagazine, ammoItemInInventory.Count, fireArmData.magazineCapacity);
            }
        }
        
        
        [Header("Range Control")]
        public bool alwaysApplyFire = false;
        
        [HideInInspector]
        public FirearmAnimation anim;
        [HideInInspector]
        [FormerlySerializedAs("audio")] public FirearmAudio faudio;
        [FormerlySerializedAs("recoil")] [HideInInspector]
        public FirearmRecoilAndSpray recoilAndSpray;
        [HideInInspector]
        public FirearmShooter shooter;
        [HideInInspector]
        public FireArmData fireArmData;

        public bool IsInitialized { get; private set; } = false;
        
        private void Awake()
        {
            anim = GetComponent<FirearmAnimation>();
            faudio = GetComponent<FirearmAudio>();
            recoilAndSpray =  GetComponent<FirearmRecoilAndSpray>();
            shooter = GetComponent<FirearmShooter>();
            fireArmData = new FireArmData();
            fireArmData.Init(this, preset);
            
            if (!preset)
            {
                Debug.LogError("[FirearmController] Preset not found, default preset loaded");
            }
            
            anim.Init(this);
            faudio.Init(this);
            recoilAndSpray.Init(this);
            shooter.Init(this);
            OwnerController = GetComponentInParent<PlayerController>();
            Owner = OwnerController.GetComponent<IDamageable>();
            
            requiredAmmoType = new AmmoType(EAmmoType.R556, 25, 0);
            ammoItemInInventory = new AmmoItem(requiredAmmoType, fireArmData.initialAmmo - fireArmData.magazineCapacity, 1, 1);
        }

        // 초기화 및 설정
        private void Start()
        {
            // 총구위치 설정
            // set muzzle pos
            if (!muzzle)
            {
                Debug.LogError("Muzzle transform is not assigned. Defaulting to the firearm's transform.", gameObject);
                muzzle = transform;
            }
            
            // 총기 상태 설정
            // set firearm state
            FirearmState = EFirearmStates.None;
            FirearmAimState = EFirearmAimStates.LowReady;
            FirePrevented = false;
            
            // 총기 탄약 설정
            // setting ammo
            //TODO 현재는 돌격 소총만 있어서 여기에 쓰는데 나중에 화기별로 따로 만들면 
            // 자식 클래스에서 수정하자
            
            var ammoInInvCnt = fireArmData.initialAmmo - fireArmData.magazineCapacity;
            AmmoInMagazine = fireArmData.magazineCapacity;
            DecalDirection = EVector3Direction.Forward;
            OnFirearmNameChanged?.Invoke(fireArmData.firearmName);

            IsInitialized = true;
        }
        

        /// <summary>
        /// 화기의 상태를 업데이트 (재장전, 탄약, 애니메이션 상태)
        /// Update Firearms's state
        /// </summary>
        private void Update()
        {
            if (HeadManager.Game.IsPaused) return;
            UpdateStatusWithInput();
            anim.UpdateAnim();
            shooter.UpdateFire();
            recoilAndSpray.UpdateSpray();
            UpdateUI();
        }

        private void UpdateUI()
        {
            // 무기 탄약 -> shooter, anim
            // 무기 이름 -> 여기
            // 크로스헤어
            if (HeadManager.UI.SceneUI is UIGameScene)
            {
                var temp = (UIGameScene)HeadManager.UI.SceneUI;
                temp.SetCrossHairProgress(anim.AimingAnimation.Progress);
                temp.SetCrosshairSpray(recoilAndSpray.GetCurrentSpray());
            }
            
        }

        /// <summary>
        /// 입력에 기반해서 각종 상태를 업데이트
        /// Based on Input, update various status;
        /// </summary>
        private void UpdateStatusWithInput()
        {
            if (FirearmState != EFirearmStates.Reloading && AmmoInMagazine > 0)
            {
                // Auto mode
                if (fireArmData.firingMode == EFiringMode.Auto)
                {
                    if (HeadManager.Input.State.FireHeld)
                    {
                        FirearmState = EFirearmStates.Fire;
                    }
                    else
                    {
                        FirearmState = EFirearmStates.None;
                    }
                    // : 뒷부분을 FirearmState로 두면 한번 클릭에 2번나가는 효과 발생
                }
                // Semi auto or burst
                else if (fireArmData.firingMode is EFiringMode.SemiAuto or EFiringMode.Burst)
                {
                    FirearmState = HeadManager.Input.State.FirePressed ? FirearmState = EFirearmStates.Fire : FirearmState;
                }
            }
            
            if (AmmoInMagazine == 0 && FirearmState != EFirearmStates.Reloading)
                FirearmState = EFirearmStates.None;

            if (HeadManager.Input.State.ReloadPressed && AmmoInMagazine != fireArmData.magazineCapacity && ammoItemInInventory.Count != 0)
            {
                FirearmState = EFirearmStates.Reloading;
            }
        }
        
        /// <summary>
        /// 산탄총처럼 장전중에 사격이 가능한 총기에 사용되는 함수
        /// 장전중에 사격을 하기위해 장전을 취소하는 역할
        /// </summary>
        public void CancelReload()
        {
            
        }
        
        private void UpdateReload()
        {
            
        }

        private void OnEnable()
        {
            FirearmState = EFirearmStates.None;
        }
    }
}