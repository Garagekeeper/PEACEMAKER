using System;
using Resources.Script.Animation;
using Resources.Script.Animation.Modifier;
using Resources.Script.Audio;
using Resources.Script.Controller;
using Resources.Script.Managers;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Firearm
{
    public class FirearmAnimation : MonoBehaviour
    {
        /// <summary>
        /// FireArmController That own this class
        /// </summary>
        public FirearmController FireArm { get; private set; }

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
        private GameObject _magazine;
        private SFXSource _reloadSound;

        private WaveAnimationModifier WalkingWaveAnimationModifier { get; set; }
        private float _defaultAimingTime;
        [HideInInspector] public Animator animator;
        private bool _isReloading;

        public void Init(FirearmController controller)
        {
            FireArm = controller;
            ProceduralAnimator = GetComponentInChildren<ProceduralAnimator>();
            animator = GetComponentInChildren<Animator>();

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
                _magazine = GameObject.Find("Magazine");

                // Set default aiming time
                if (AimingAnimation) _defaultAimingTime = AimingAnimation.length;

                if (WalkingAnimation)
                    WalkingWaveAnimationModifier = WalkingAnimation.GetComponent<WaveAnimationModifier>();

                // Set up listeners for character manager events
                if (FireArm.Owner != null)
                {
                    if (JumpAnimation)
                        FireArm.Owner.onJump.AddListener(() => JumpAnimation.Play(0));

                    if (LandAnimation)
                        FireArm.Owner.onLand.AddListener(() => JumpAnimation.Play(0));
                }

                // TODO 아무리 찾아봐도 이 애니메이션이 없는데
                ReloadingAnimation = ProceduralAnimator.GetAnimation("Reloading");
                _isReloading = false;
            }
        }

        public void UpdateAnim()
        {
            UpdateProceduralAnimation();
            UpdateAnimation();
        }

        private void UpdateProceduralAnimation()
        {
            if (WalkingWaveAnimationModifier != null)
            {
                // Update walking wave animation based on character velocity
                float characterVelocity = FireArm.Owner.CharacterController.velocity.magnitude;
                WalkingWaveAnimationModifier.speedMultiplier = Mathf.Lerp(WalkingWaveAnimationModifier.speedMultiplier,
                    characterVelocity, Time.deltaTime * 5);

                if (FireArm.Owner.CharacterController.velocity.magnitude > 1 &&
                    FireArm.Owner.CharacterController.isGrounded)
                    WalkingWaveAnimationModifier.scaleMultiplier = Mathf.Lerp(
                        WalkingWaveAnimationModifier.scaleMultiplier, characterVelocity, Time.deltaTime * 5);
                else
                    WalkingWaveAnimationModifier.scaleMultiplier =
                        Mathf.Lerp(WalkingWaveAnimationModifier.scaleMultiplier, 0, Time.deltaTime * 5);
            }

            if (SwayAnimation)
            {
                SwayAnimation.SwayAnimationModifiers[0].InputX = (SystemManager.Input.Look.x / Time.deltaTime) * 0.5f;
                SwayAnimation.SwayAnimationModifiers[0].InputY = (SystemManager.Input.Look.y / Time.deltaTime) * 0.5f;
            }

            if (SwayAimingAnimation)
            {
                SwayAimingAnimation.SwayAnimationModifiers[0].InputX =
                    (SystemManager.Input.Look.x / Time.deltaTime) * 0.5f;
                SwayAimingAnimation.SwayAnimationModifiers[0].InputY =
                    (SystemManager.Input.Look.y / Time.deltaTime) * 0.5f;
            }

            if (SprintingAnimation != null)
            {
                SprintingAnimation.triggerType = ProceduralAnimation.InputActionType.None;
                SprintingAnimation.IsPlaying = SystemManager.Input.SprintPressed && 
                                               (SystemManager.Input.Move != Vector2.zero) && FireArm.FirearmState == EFirearmStates.None;
            }


            if (AimingAnimation != null)
            {
                // Adjust aiming animation speed based on firearm aim speed
                //TODO 부착물에 따른 MOD
                //if (false)
                //AimingAnimation.length = _defaultAimingTime / 1;

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
        
        
        private void UpdateAnimation()
        {
            if (FireArm.FirearmState == EFirearmStates.Reloading)
            {
                UpdateReload();
            }
            animator.SetInteger("Ammo", FireArm.currentAmmo.Count);
            animator.SetFloat("ADS Amount", AimingAnimation.Progress);
        }
        
        private void UpdateReload()
        {
            //condition check
            // 남은 탄약이 없으면 종료
            if (FireArm.currentAmmo.Count == 0) return;
            // 이미 탄창에 탄약이 가득찬 경우 종료
            if (FireArm.AmmoInMagazine >= FireArm.fireArmData.magazineCapacity) return;
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            // 0.이미 재장전 애니메에션을 재생중
            if (_isReloading)
            {
                // 애니메이션의 재생이 끝나면
                if (info.IsName("Reload") && info.normalizedTime >= 1f)
                {
                    // 탄창에 들어가는 총알의 수를 계산
                    // 현재 탄창에 남은건 버리는 방향으로 설계
                    var inventory2Magazine = Mathf.Min(FireArm.fireArmData.magazineCapacity, FireArm.currentAmmo.Count);
                    FireArm.AmmoInMagazine = inventory2Magazine;
                    FireArm.currentAmmo.Count -=  inventory2Magazine;
                    _isReloading = false;
                    animator.SetBool("Is Reloading", _isReloading);
                    FireArm.FirearmState =  EFirearmStates.None;
                }
            }
            // 1. 애니메이션이 재생중이 아님
            else
            {
                //Start Process
                //TODO Play Reload Sound
               // SystemManager.Audio.PlayWithPreset(FireArm.fireArmData.reloadSoundPreset, _magazine.transform);
               _reloadSound = SystemManager.Audio.PlayWithPreset(FireArm.fireArmData.reloadSoundPreset,
                    FireArm.Owner.transform);
                
                // 1-0. 코드에 의해 조절되는 재장전 (산탄총) 
                if (FireArm.fireArmData.reloadType == EReloadType.Scripted)
                {
                
                }
                // 1-1. 일반적인 재장전 애니메이션
                _isReloading = true;
                animator.SetBool("Is Reloading", _isReloading);
            }
        }

        private void OnEnable()
        {
            _isReloading = false;
        }

        private void OnDisable()
        {
            SystemManager.Audio.ReturnSFXToPool(_reloadSound);
        }
    }
}