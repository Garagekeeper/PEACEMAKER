using System;
using System.Collections.Generic;
using Akila.FPSFramework;
using Resource.Script.Ammo;
using Resource.Script.Animation;
using Resource.Script.Animation.Modifier;
using Resource.Script.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using static Resource.Script.Defines;
using static Resource.Script.Utilities;
using Object = UnityEngine.Object;

namespace Resource.Script.Controller
{
    public class FirearmController : MonoBehaviour
    {
        [Tooltip("The Transform from which the shots are fired.")]
        public Transform muzzle;

        [Tooltip("The Transform from which shell casings are ejected.")]
        public Transform casingEjectionPort;

        [Tooltip("Particle system effects that play when chambering a bullet.")]
        public ParticleSystem chamberingEffects;

        [Tooltip("Events related to this firearm."), Space]
        public FirearmEvents firearmEvents;
        
        public Camera mainCam;
     
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
        
        /// <summary>
        /// Tracks the delay between shots fired.
        /// </summary>
        private float fireTimer;
       
        public enum EFirearmStates
        {
            None = 0,
            Reloading = 1,
            Fire = 2,
        };

        public enum EFirearmAimStates
        {
            LowReady = 0,
            ShoulderReady,
        };
        
        public EFirearmStates FirearmState { get; set; }
        public EFirearmAimStates FirearmAimState { get; set; }
        public EVector3Direction DecalDirection { get; set; }
        
        
        public ProceduralAnimator ProceduralAnimator { get; set; }
        public ProceduralAnimation FiringAnimation { get; private set; }
        public ProceduralAnimation AimFiringAnimation { get; private set; }
        public ProceduralAnimation AimingAnimation { get; protected set; }
        public ProceduralAnimation BreathingAnimation { get; private set; }
        public ProceduralAnimation BreathingAimAnimation { get; private set; }
        public ProceduralAnimation WalkingAnimation { get; private set; }
        public ProceduralAnimation SprintingAnimation { get; private set; }
        public ProceduralAnimation TacticalSprintingAnimation { get; private set; }
        public ProceduralAnimation RecoilAnimation { get; private set; }
        public ProceduralAnimation RecoilAimAnimation { get; private set; }
        public ProceduralAnimation JumpAnimation { get; private set; }
        public ProceduralAnimation LandAnimation { get; private set; }
        public ProceduralAnimation LeanRightAnimation { get; private set; }
        public ProceduralAnimation LeanLeftAnimation { get; private set; }
        public ProceduralAnimation LeanRightAimAnimation { get; private set; }
        public ProceduralAnimation LeanLeftAimAnimation { get; private set; }
        public ProceduralAnimation CrouchAnimation { get; private set; }
        public ProceduralAnimation SwayAnimation { get; private set; }
        public ProceduralAnimation SwayAimingAnimation { get; private set; }
        public ProceduralAnimation ReloadingAnimation { get; private set; }
        
        private WaveAnimationModifier WalkingWaveAnimationModifier { get; set; }
        
        private float _defaultAimingTime;
        
        public Animator animator;
        private CharacterController _characterController;
        
        /***************************
         * common stat/variable
         **************************/
        public GameObject defaultDecalPrefab;
        
        [HideInInspector]
        public bool tracerRounds = true;
        [HideInInspector]
        public float projectileSize = 0.01f;
        [HideInInspector]
        public float muzzleVelocity = 250;
        [HideInInspector]
        public float decalSize = 1;
        [HideInInspector]
        public float impactForce = 10;
        [HideInInspector]
        public float shotDelay = 0;
        [HideInInspector]
        public float horizontalRecoil = 0.7f;
        [HideInInspector]
        public float verticalRecoil = 0.1f;
        
        //Recoil
        [HideInInspector]
        public float cameraRecoil = 1;
        [HideInInspector]
        public float cameraShakeAmount = 0.05f;
        [HideInInspector]
        public float cameraShakeRoughness = 5;
        [HideInInspector]
        public float cameraShakeStartTime = 0.1f;
        [HideInInspector]
        public float cameraShakeDuration = 0.1f;
        protected LayerMask mask;
        
        protected bool _canCancelReload = false;
        public ProjectileController projectilePrefab;
        public List<ProjectileController> Projectiles { get; set; }
        public PlayerController Owner { get; set; }

        public enum EFiringMode
        {
            SemiAuto = 0,
            Auto = 1,
            Burst = 2,
        }

        public enum EShotMechanism
        {
            Projectile = 0,
            HitScan = 1,
        }
        
        public EFiringMode FiringMode {get; private set;}
        public EShotMechanism ShotMechanism { get; set; }
        
        public enum ReloadType
        {
            Default = 0,
            /// <summary>
            /// 재장전 시 모든 동작이 코드에 의해서 통제됨
            /// 주로 장전 중에 발사가 가능한 샷건 종류에서 사용
            /// All of actions are controlled by script
            /// useful for shotgun(Fire while reloading)
            /// </summary>
            Scripted = 1
        }
        ReloadType reloadType;

        protected float fireRate;
        private Vector3 _currentFirePosition;
        private Quaternion _currentFireRotation;
        private Vector3 _currentFireDirection;
        private Vector3 _originalFireDirection;
        private int _currentSprayPointIdex;
        
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
        public AmmoItem currentAmmo;

        public FirearmController(ProceduralAnimation recoilAnimation)
        {
            RecoilAnimation = recoilAnimation;
        }

        /// <summary>
        /// The maximum capacity of the magazine.
        /// 재장전하면 탄창에 있는 총알은 버려짐
        /// </summary>

        /// <summary>
        /// 탄창의 크기
        /// </summary>
        public int MagazineCapacity { get; set; }
        
        /// <summary>
        /// 현재 장전된 탄창 내부의 탄약 수
        /// </summary>
        public int AmmoInMagazine { get; set; }
        
        [Header("Range Control")]
        public float range = 300;
        public LayerMask hittableLayers = -1;
        public bool alwaysApplyFire = false;

        private void Awake()
        {
            mainCam = GetMainCamera();
            mask |= LayerMask.GetMask("Default");
            mask |= LayerMask.GetMask("TransparentFX");
            mask |= LayerMask.GetMask("Ignore Raycast");
            mask |= LayerMask.GetMask("Water");
            mask |= LayerMask.GetMask("UI");
            mask |= LayerMask.GetMask("FPS Object");
            mask |= LayerMask.GetMask("Enviroment");
            mask |= LayerMask.GetMask("Iteractable");
            
            Owner = GetComponentInParent<PlayerController>();
            ProceduralAnimator = GetComponentInChildren<ProceduralAnimator>();
            animator = GetComponentInChildren<Animator>();
            _characterController = GetComponentInParent<CharacterController>();
            defaultDecalPrefab = Resources.Load<GameObject>("@Prefabs/Particle Systems/Bullet Impact/Stone Impact.prefab");
            
            // Initialize animations if proceduralAnimator exists
            if (ProceduralAnimator != null)
            {
                FiringAnimation = ProceduralAnimator.GetAnimation("Recoil");
                AimFiringAnimation = ProceduralAnimator.GetAnimation("Aim Recoil");
                BreathingAnimation = ProceduralAnimator.GetAnimation("Breathing");
                BreathingAimAnimation = ProceduralAnimator.GetAnimation("Breathing Aim");
                WalkingAnimation = ProceduralAnimator.GetAnimation("Walking");
                SprintingAnimation = ProceduralAnimator.GetAnimation("Sprinting");
                TacticalSprintingAnimation = ProceduralAnimator.GetAnimation("Tactical Sprinting");
                AimingAnimation = ProceduralAnimator.GetAnimation("Aiming");
                RecoilAnimation = ProceduralAnimator.GetAnimation("Recoil");
                RecoilAimAnimation = ProceduralAnimator.GetAnimation("Recoil Aim");
                JumpAnimation = ProceduralAnimator.GetAnimation("Jump");
                LandAnimation = ProceduralAnimator.GetAnimation("Land");
                LeanRightAnimation = ProceduralAnimator.GetAnimation("Lean Right");
                LeanLeftAnimation = ProceduralAnimator.GetAnimation("Lean Left");
                LeanRightAimAnimation = ProceduralAnimator.GetAnimation("Lean Right Aim");
                LeanLeftAimAnimation = ProceduralAnimator.GetAnimation("Lean Left Aim");
                CrouchAnimation = ProceduralAnimator.GetAnimation("Crouch");
                SwayAnimation = ProceduralAnimator.GetAnimation("Sway");
                SwayAimingAnimation = ProceduralAnimator.GetAnimation("Sway Aiming");

                // Set default aiming time
                if (AimingAnimation) _defaultAimingTime = AimingAnimation.length;
                
                if (WalkingAnimation)
                    WalkingWaveAnimationModifier = WalkingAnimation.GetComponent<WaveAnimationModifier>();
                
                // Set up listeners for character manager events
                if (Owner != null)
                {
                    if (JumpAnimation)
                        Owner.onJump.AddListener(() => JumpAnimation.Play(0));

                    if (LandAnimation)
                        Owner.onLand.AddListener(() => JumpAnimation.Play(0));
                }
                
                // TODO 아무리 찾아봐도 이 애니메이션이 없는데
                ReloadingAnimation = ProceduralAnimator.GetAnimation("Reloading");
            }
        }

        // 초기화 및 설정
        private void Start()
        {
            // TODO 여기에있는 설정은 돌격소총용, 나머지는 따로 빼서 데이터를 스크립터블로 하던가.
            // 아니면 런타임에 지정해서 불러오던가.
            reloadType = ReloadType.Default;
            
            // 총구위치 설정
            // set muzzle pos
            if (!muzzle)
            {
                Debug.LogError("Muzzle transform is not assigned. Defaulting to the firearm's transform.", gameObject);
                muzzle = transform;
            }
            
            // 탄피배출구 설정
            // set ejection port
            // if (!casingEjectionPort)
            // {
            //     Debug.LogError("Casing ejection port transform is not assigned. Defaulting to the firearm's transform.", gameObject);
            //     casingEjectionPort = transform;
            // }
            
            //cam 설정
            cameraRecoil = 1;
            cameraShakeAmount = 0.05f;
            cameraShakeRoughness = 5;
            cameraShakeStartTime = 0.1f;
            cameraShakeDuration = 0.1f;
            
            // 총기 상태 설정
            // set firearm state
            FirearmState = EFirearmStates.None;
            FirearmAimState = EFirearmAimStates.LowReady;
            FirePrevented = false;
            FiringMode = EFiringMode.Auto;
            fireRate = 850;
            ShotMechanism = EShotMechanism.HitScan;
            impactForce = 10;
            shotDelay = 0;
            horizontalRecoil = 0.7f;
            verticalRecoil = 0.1f;
            range = 300;
            
            // 총기 탄약 설정
            // setting ammo
            //TODO 현재는 돌격 소총만 있어서 여기에 쓰는데 나중에 화기별로 따로 만들면 
            // 자식 클래스에서 수정하자
            requiredAmmoType = new AmmoType(EAmmoType.R556, 25, 0);
            currentAmmo = new AmmoItem(requiredAmmoType, 30, 1, 1);
            MagazineCapacity = 30;
            AmmoInMagazine = MagazineCapacity;
            tracerRounds = true;
            projectileSize = 0.01f;
            muzzleVelocity = 250;
            decalSize = 1;
            hittableLayers = -1;
            DecalDirection = EVector3Direction.Forward;
        }
        
        private void UpdateLeanAnimations()
        {
            // Update right and left leaning animations
            if (LeanRightAnimation != null)
            {
                LeanRightAnimation.IsPlaying = SystemManager.Input.LeanRightInput;
            }
            
            if (LeanLeftAnimation != null)
            {
                LeanLeftAnimation.IsPlaying = SystemManager.Input.LeanLeftInput;
            }
            
            // Update right and left aiming lean animations
            if (LeanRightAimAnimation != null)
            {
                LeanRightAimAnimation.IsPlaying = SystemManager.Input.LeanRightInput;
            }
            
            if (LeanLeftAimAnimation != null)
            {
                LeanLeftAimAnimation.IsPlaying = SystemManager.Input.LeanLeftInput;
            }
        }

        /// <summary>
        /// 화기의 상태를 업데이트 (재장전, 탄약, 애니메이션 상태)
        /// Update Firearms's state
        /// </summary>
        private void Update()
        {
            UpdateAnim();
            UpdateStatusWithInput();
            UpdateFire();
            UpdateReload();
        }

        public void UpdateAnim()
        {
             if (WalkingWaveAnimationModifier != null)
             {
                 // Update walking wave animation based on character velocity
                 float characterVelocity = Owner.CharacterController.velocity.magnitude;
                 WalkingWaveAnimationModifier.speedMultiplier = Mathf.Lerp(WalkingWaveAnimationModifier.speedMultiplier, characterVelocity, Time.deltaTime * 5);

                 if (Owner.CharacterController.velocity.magnitude > 1 && Owner.CharacterController.isGrounded)
                     WalkingWaveAnimationModifier.scaleMultiplier = Mathf.Lerp(WalkingWaveAnimationModifier.scaleMultiplier, characterVelocity, Time.deltaTime * 5);
                 else
                     WalkingWaveAnimationModifier.scaleMultiplier = Mathf.Lerp(WalkingWaveAnimationModifier.scaleMultiplier, 0, Time.deltaTime * 5);
             }

             if(SwayAnimation)
             {
                 SwayAnimation.SwayAnimationModifiers[0].InputX = (SystemManager.Input.Look.x / Time.deltaTime) * 0.5f;
                 SwayAnimation.SwayAnimationModifiers[0].InputY = (SystemManager.Input.Look.y / Time.deltaTime) * 0.5f;
             }

             if (SwayAimingAnimation)
             {
                 SwayAimingAnimation.SwayAnimationModifiers[0].InputX = (SystemManager.Input.Look.x / Time.deltaTime) * 0.5f;
                 SwayAimingAnimation.SwayAnimationModifiers[0].InputY = (SystemManager.Input.Look.y / Time.deltaTime) * 0.5f;
             }

             if (SprintingAnimation != null)
             {
                 SprintingAnimation.triggerType = ProceduralAnimation.InputActionType.None;
                 SprintingAnimation.IsPlaying = SystemManager.Input.SprintPressed & !SystemManager.Input.FireHeld &&
                                                (SystemManager.Input.Move != Vector2.zero);
                 //Debug.Log(SystemManager.Input.SprintPressed & SystemManager.Input.FireHeld);
                 //SprintingAnimation.IsPlaying = SystemManager.Input.SprintPressed;
             }

             //TODO sprint doubletap 적용
             if (TacticalSprintingAnimation != null)
             {
                 //TacticalSprintingAnimation.triggerType = ProceduralAnimation.InputActionType.None;
                 //TacticalSprintingAnimation.IsPlaying = characterInput.TacticalSprintInput;
             }

             if (AimingAnimation != null)
             {
                 // Adjust aiming animation speed based on firearm aim speed
                 //TODO 부착물에 따른 MOD
                 if (false)
                     AimingAnimation.length = _defaultAimingTime / 1;

                 AimingAnimation.triggerType = ProceduralAnimation.InputActionType.None;
                 AimingAnimation.IsPlaying = SystemManager.Input.AimHeld;
             }

             // Update crouch animation state
             if (CrouchAnimation)
             {
                 CrouchAnimation.triggerType = ProceduralAnimation.InputActionType.None;
                 CrouchAnimation.IsPlaying = SystemManager.Input.CrouchToggle;
             }

             // Update firearm-related animations based on reload and firing state
 
             //TODO 추후 적용
             // if (isReloading || attemptingToFire)
             // {
             //     //TODO 부착물에 따른 MOD
             //     if (RecoilAnimation != null) RecoilAnimation.weight = 1;
             //     if (RecoilAimAnimation != null) RecoilAnimation.weight = 1;
             //
             //     if (SprintingAnimation != null) SprintingAnimation.AlwaysStayIdle = true;
             //     if (TacticalSprintingAnimation != null) TacticalSprintingAnimation.AlwaysStayIdle = true;
             // }
             // else
             // {
             //     if (SprintingAnimation != null) SprintingAnimation.AlwaysStayIdle = false;
             //     if (TacticalSprintingAnimation != null) TacticalSprintingAnimation.AlwaysStayIdle = false;
             // }
             

             // Handle leaning animations
             UpdateLeanAnimations();
        }

        /// <summary>
        /// 입력에 기반해서 각종 상태를 업데이트
        /// Based on Input, update various status;
        /// </summary>
        private void UpdateStatusWithInput()
        {
            // Auto mode
            if (FiringMode == EFiringMode.Auto)
            {
                // : 뒷부분을 FirearmState로 두면 한번 클릭에 2번나가는 효과 발생
                FirearmState = SystemManager.Input.FireHeld ? FirearmState = EFirearmStates.Fire : FirearmState = EFirearmStates.None;
            }
            // Semi auto or burst
            else if (FiringMode is EFiringMode.SemiAuto or EFiringMode.Burst)
            {
                FirearmState = SystemManager.Input.FirePressed ? FirearmState = EFirearmStates.Fire : FirearmState;
            }
        }
        
        private void UpdateFire()
        {
            if (FirePrevented) return;

            if (FirearmState != EFirearmStates.Fire) return;

            Fire();

        }

        public void Fire()
        {
            var firePosition = Vector3.zero;
            var fireRotation = Quaternion.identity;
            var fireDirection = Vector3.zero;
            
            // Muzzle to CamForward
            // 총구에서 카메라 중앙으로 투사체 발사.
            // 정확한 에임
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out var hitInfo, mask))
            {
                // 적정거리 이상으로 떨어져 있는 경우, 총구에서 시작해서 화면 정중앙으로 
                if (hitInfo.distance > 5f)
                {
                    firePosition = muzzle.position;
                    fireRotation = muzzle.rotation;
                    fireDirection = (hitInfo.point - muzzle.position).normalized;
                }
                // 너무 가까운 경우 카메라에서 시작해서 총구 방향으로
                else
                {
                    firePosition = mainCam.transform.position;
                    fireRotation = mainCam.transform.rotation;
                    fireDirection = muzzle.forward;
                }
            }
            
            FireInternal(firePosition, fireRotation, fireDirection);
        }

        private void FireInternal(Vector3 firePosition, Quaternion fireRotation, Vector3 fireDirection)
        {
            // RPM보다 업데이트의 갱신이 빠르면 종료
            if (Time.time <= fireTimer) return;

            // 줍기 뽑기등의 애니메이션을 재생중이 아니다.
            if (IsPlayingRestrictedAnimation() == false)
            {
                ShotsFired = 0;
                //TODO 여기서 반동(recoil) 적용  
                //반동 적용
                ApplyRecoil();
                _originalFireDirection = fireDirection;
            }
            
            FireDone(firePosition, fireRotation, fireDirection);
            fireTimer = Time.time + (60f/fireRate);
        }
        
        /// <summary>
        /// 제한된 애니메이션을 플레이하고 있는지 확인, 줍기 뽑기 등의 행동중에는 재장전 사격 불가
        /// Checks if any of the animators are currently playing a restricted animation.
        /// </summary>
        /// <returns></returns>
        private bool IsPlayingRestrictedAnimation()
        {
            return animator.CurrentPlayingAnim("Take") || animator.CurrentPlayingAnim("Pickup");
            //TODO 나중에 에디터에서 리스트를 받아오던거 해서 처리
        }

        /// <summary>
        /// 발사의 마지막 단계 (탄소모 / 이펙트 / 데미지 처리)
        /// </summary>
        void FireDone(Vector3 position, Quaternion rotation, Vector3 direction)
        {
            _currentFirePosition = position;
            _currentFireRotation = rotation;
            //TODO 여기서 반동(스프레이) 구현
            
            _currentFireDirection = direction;
            
            //TODO 멀티플레이 적용시 이벤트형식으로 변경
            ProcessShotHit(direction);

            //ApplyFireOnce();
            ShotsFired++;
            
            /*
             * shotsFired++
                첫 번째 탄은 이미 FireDone 안에서 발사됨.
                만약 shotCount > 1 이면(즉, 1번 트리거로 여러 발 쏘는 무기면)
                “두 번째 탄, 세 번째 탄 …을 발사해야 하는지 체크”
                shotDelay가 0 이하이면 즉시 다음 발 발사
                샷건처럼 “동시에 여러 펠릿”이 나가는 무기
                
                shotDelay > 0이면 Invoke로 지연 발사
                예: 버스트 모드(3점사)
                예: 기관총 중 특정 무기에서 '따닥 따닥' 같은 템포를 만드는 용도
                예: 레일건 등에서 '충전 후 연속 발사'
             */
            //TODO 점사
            // if (shotsFired < preset.shotCount && remainingAmmoCount > 0)
            // {
            //     if (preset.shotDelay <= 0f)
            //     {
            //         FireDone(position, rotation, currentFireDirection);
            //     }
            //     else if (shotsFired >= 1)
            //     {
            //         Invoke(nameof(InvokeFireDone), preset.shotDelay);
            //     }
            // }

        }

        /// <summary>
        /// 투사체 or Hitscan처리
        /// </summary>
        /// <param name="direction"></param>
        private void ProcessShotHit(Vector3 direction)
        {
            //0. HitScan
            if (ShotMechanism == EShotMechanism.HitScan)
            {
                
                Ray ray = new Ray(muzzle.position, direction);

                if (Physics.Raycast(ray, out RaycastHit hit, range, hittableLayers))
                {
                    float calculatedDmg = currentAmmo.GetAmmoDmg();
                    float finalDmg = alwaysApplyFire ? calculatedDmg : calculatedDmg / currentAmmo.ammo.BulletCountOnce;
                    UpdateHits(defaultDecalPrefab, ray, hit, finalDmg, DecalDirection);
                }
            }
            //1. Projctile
            else if (ShotMechanism == EShotMechanism.Projectile)
            {
                
            }

            FirearmState = EFirearmStates.None;
        }

        public ProjectileController SpawnProjectile(Vector3 position, Quaternion rotation, Vector3 direction, float speed, float range)
        {

            // Check if the preset or projectile prefab is null
            if (projectilePrefab == null)
            {
                Debug.LogError("Projectile prefab is not set. default prefab Instantiate");
                projectilePrefab = Resources.Load<ProjectileController>("@Prefabs/Weapons/Projectile");
                return null;
            }

            // Instantiate the projectile prefab at the given position and rotation
            ProjectileController newProjectileController = Instantiate(projectilePrefab, position, rotation);

            // Initialize the velocity of the projectile to zero
            Vector3 initialVelocity = Vector3.zero;

            // If a character manager exists, add its velocity to the initial velocity of the projectile
            if (_characterController)
            {
                initialVelocity += _characterController.velocity;
            }

            // Set up the new projectile with the given parameters: owner, direction, initial velocity, speed, and range
            newProjectileController.Setup(this, direction, initialVelocity, speed, range);

            // Return the newly spawned projectile
            return newProjectileController;
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
        
          /// <summary>
        /// Updates the state of objects hit by a projectile, including applying damage, handling decals, and applying forces.
        /// </summary>
        /// <param name="defaultDecal">The default decal to apply on the hit surface.</param>
        /// <param name="ray">The ray that represents the projectile's path.</param>
        /// <param name="hit">Information about the hit result.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="decalDirection">The direction for orienting the decal.</param>
        public void UpdateHits(GameObject defaultDecal, Ray ray, RaycastHit hit, float damage, EVector3Direction decalDirection)
        {
            if(_characterController == null)
            {
                Debug.LogError("Character (ICharacterController) in the firearm is not set.");
                return;
            }

            var creature = _characterController.GetComponent<Creature.Creature>();

            // Invoke hit callbacks for the firearm
            InvokeHitCallbacks(_characterController.gameObject, ray, hit);

            // Exit if the hit target is the same as the character
            if (hit.transform == _characterController.transform) return;

            float damageMultiplier = 1;

            // Handle damageable groups
            // 데미지를 받을 수 있는 부위
            //TODO damageablePart 옮기기
            if (hit.transform.TryGetComponent(out IDamageablePart damageablePart))
            {
                // 뒤에 1 부착물 mod
                damageMultiplier = damageablePart.damageMultipler * 1f;
            }

            // 데미지를 받는 객체
            IDamageable damageable = hit.transform.FindSelfChildParent<IDamageable>();

            // Handle damageable objects
            if (damageable is { Health: > 0 })
            {
                var totalDamage = damage * damageMultiplier;

                GameObject creatureGo = null;

                if (creature) creatureGo = creature.gameObject;

                damageable.Damage(totalDamage, creatureGo);

                bool shouldHighlight = damageable.Health <= 0;

                if (_characterController != null)
                {
                    // 히트마커 표시
                    // TODO 히트마커 옮기기
                    UIManager uiManager = UIManager.Instance;

                    if (uiManager != null)
                    {
                        Hitmarker hitmarker = uiManager.Hitmarker;
                        hitmarker?.Show(shouldHighlight);
                    }
                }
            }

            // 총알 자국 처리
            // Handle custom decals
            if (hit.transform.TryGetComponent(out CustomDecal customDecal))
            {
                defaultDecal = customDecal.decalVFX;
            }

            // Exit if the hit target is the same as the character manager
            if (Owner?.transform == hit.transform) return;

            // Apply default or custom decal
            if (defaultDecal)
            {
                Vector3 hitPoint = hit.point;
                Quaternion decalRotation = GetHitRotation(hit);
                GameObject decalInstance = Instantiate(defaultDecal, hitPoint, decalRotation);

                decalInstance.transform.localScale *= decalSize;
                decalInstance.transform.SetParent(hit.transform);

                float decalLifetime = customDecal?.lifeTime ?? 60f;
                Destroy(decalInstance, decalLifetime);
            }

            // 맞은 물체에 RB가 있으면 피격되서 흔들리도록 모션을 준다.
            // Apply force to the rigidbody if present
            if (hit.rigidbody)
            {
                var impact = shotDelay <= 0f
                    ? (impactForce / requiredAmmoType.BulletCountOnce)
                    : impactForce;

                hit.rigidbody.AddForceAtPosition(-hit.normal * impact, hit.point, ForceMode.Impulse);
            }
        }
         
        //TODO 무슨 함수인지 잘 알아보기
        /// <summary>
        /// Invokes hit-related callbacks on the hit object, as well as its children and parents, if applicable.
        /// </summary>
        /// <param name="sourcePlayer">The player responsible for the hit.</param>
        /// <param name="ray">The ray that caused the hit.</param>
        /// <param name="hit">Information about the hit.</param>
        public static void InvokeHitCallbacks(GameObject sourcePlayer, Ray ray, RaycastHit hit)
        {
            if (hit.transform.TryGetComponent<Ignore>(out Ignore _ignore) && _ignore.ignoreHitDetection) return;

            // Create a HitInfo object to store details about the hit
            HitInfo hitInfo = new HitInfo(sourcePlayer, hit, ray.origin, ray.direction);

            // Retrieve the GameObject that was hit
            GameObject obj = hit.transform.gameObject;

            // Try to get the IOnAnyHit interface implementation on the hit object, its children, and its parent
            IOnAnyHit onAnyHit = obj.transform.GetComponent<IOnAnyHit>();
            IOnAnyHitInChildren onAnyHitInChildren = obj.transform.GetComponentInParent<IOnAnyHitInChildren>();
            IOnAnyHitInParent onAnyHitInParent = obj.transform.GetComponentInChildren<IOnAnyHitInParent>();

            // Try to get the IOnHit interface implementation on the hit object, its children, and its parent
            IOnHit onHit = obj.transform.GetComponent<IOnHit>();
            IOnHitInChildren onHitInChildren = obj.transform.GetComponentInParent<IOnHitInChildren>();
            IOnHitInParent onHitInParent = obj.transform.GetComponentInChildren<IOnHitInParent>();

            // Invoke the OnHit method on the IOnHit interface, if implemented
            onHit?.OnHit(hitInfo);

            // Invoke the OnHitInChildren method on the IOnHitInChildren interface, if implemented
            onHitInChildren?.OnHitInChildren(hitInfo);

            // Invoke the OnHitInParent method on the IOnHitInParent interface, if implemented
            onHitInParent?.OnHitInParent(hitInfo);

            // Invoke the OnAnyHit method on the IOnAnyHit interface, if implemented
            onAnyHit?.OnAnyHit(hitInfo);

            // Invoke the OnAnyHitInChildren method on the IOnAnyHitInChildren interface, if implemented
            onAnyHitInChildren?.OnAnyHitInChildren(hitInfo);

            // Invoke the OnAnyHitInParent method on the IOnAnyHitInParent interface, if implemented
            onAnyHitInParent?.OnAnyHitInParent(hitInfo);
        }
        
        /// <summary>
        /// Applies recoil effects to the weapon, including playing recoil animations and adjusting camera recoil based on the current settings.
        /// </summary>
        private void ApplyRecoil()
        {
            // Play recoil animations if the procedural animator is assigned
            if (ProceduralAnimator)
            {
                RecoilAnimation?.Play(0); // Play recoil animation from the beginning
                RecoilAimAnimation?.Play(0); // Play recoil aim animation from the beginning
            }

            // Apply camera recoil adjustments if the character manager is assigned
            if (Owner)
            {
                // Calculate the recoil values based on the preset and firearm attachments, considering aiming state
                //TODO 부착물에 따른 MOD 적용
                //float vRecoil = verticalRecoil * firearmAttachmentsManager.recoil;
                float vRecoil = verticalRecoil;
                float hRecoil = horizontalRecoil;
                float camRecoil = cameraRecoil;

                Owner.AddLookValue(vRecoil, hRecoil);

                // TODO cam 흔들리게
                // if(characterManager.cameraManager)
                // {
                //     if (characterManager.cameraManager.cameraKickAnimation)
                //     {
                //         characterManager.cameraManager.cameraKickAnimation.Play(0);
                //
                //         characterManager.cameraManager.cameraKickAnimation.weight = cameraRecoil;
                //     }
                // }
            }
        }
    }
}