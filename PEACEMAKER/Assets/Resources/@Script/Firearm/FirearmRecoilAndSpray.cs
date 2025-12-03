using Resources.Script.Animation;
using Resources.Script.Controller;
using UnityEngine;

namespace Resources.Script.Firearm
{
    public class FirearmRecoilAndSpray : MonoBehaviour
    {
        public FirearmController FireArm { get; private set; }
        private ProceduralAnimator ProcAnimator { get; set; }
        public float fireDecayDelay = 0.1f; 
  
        
        //TODO preset으로 관리
        // Hip Fire 실시간 산포
        private float currentHipSpray = 0f;
        // ADS 실시간 산포
        private float currentADSSpray = 0f;   

        // 일반 상태에서 도달할 수 있는 최대 탄 퍼짐
        public float maxHipSpray = 5.0f;
        // 조준 상태에서 도달할 수 있는 최대 탄 퍼짐
        public float maxADSSpray = 1f;

        // 회복 / 증가 속도 제어
        public float hipRampUpTime = 0.35f;
        public float ADSRampUpTime = 0.10f;
        public float hipRecoveryTime = 0.10f;
        public float ADSRecoveryTime = 0.05f;

        // Spread multipliers
        private float horizontalMultiplier = 1f;
        private float verticalMultiplier = 1f;
        
        public void Init(FirearmController fireArm)
        {
            FireArm = fireArm;
            ProcAnimator = FireArm.anim.ProceduralAnimator;
            maxHipSpray = 5f;
            hipRampUpTime = 0.1f;
            maxADSSpray = 1f;
            hipRampUpTime = 0.4f;
        }

        public void UpdateSpray()
        {
            float moveSpeed = FireArm.Owner.CharacterController.velocity.magnitude;
            float aimProgress = FireArm.anim.AimingAnimation.Progress;
            bool grounded = FireArm.Owner.CharacterController.isGrounded;
            bool recentlyFired = Time.time - FireArm.shooter.LastFireTime <= fireDecayDelay;

            // Hip Fire
            {
                float targetMaxSpray = maxHipSpray;
                // 최근에 사격을 했고, 조준중이 아니라면
                if (recentlyFired && aimProgress < 0.99f)
                {
                    // 빠르게 최대치 도달
                    float rate = targetMaxSpray / hipRampUpTime;
                    currentHipSpray = Mathf.MoveTowards(currentHipSpray, targetMaxSpray, rate * Time.deltaTime);
                }
                else
                {
                    // 회복
                    float rate = targetMaxSpray / hipRecoveryTime;
                    currentHipSpray = Mathf.MoveTowards(currentHipSpray, 0.01f, rate * Time.deltaTime);
                }

                // 이동 시 추가 스프레이를 적용
                if (moveSpeed > 0.1f && aimProgress < 0.99f)
                {
                    float normalized = Mathf.InverseLerp(0f, FireArm.Owner.sprintSpeed, moveSpeed);
                    float runningSpray = Mathf.Lerp(0.5f, 1f, normalized) * maxHipSpray;
                    currentHipSpray = Mathf.Max(currentHipSpray, runningSpray);
                }
            }

            // ADS
            {
                float targetMaxSpray = maxADSSpray;
                // 최근에 사격을 했고 조준중이라면
                if (recentlyFired && aimProgress >= 0.99f)
                {
                    float rate = targetMaxSpray / ADSRampUpTime;
                    currentADSSpray = Mathf.MoveTowards(currentADSSpray, targetMaxSpray, rate * Time.deltaTime);
                }
                else
                {
                    float rate = targetMaxSpray / ADSRecoveryTime;
                    currentADSSpray = Mathf.MoveTowards(currentADSSpray, targetMaxSpray, rate * Time.deltaTime); // ADS는 항상 최대치 유지
                }
            }
        }
        
        public void ApplyRecoil()
        {
            // Play recoil animations if the procedural animator is assigned
            if (ProcAnimator)
            {
                FireArm.anim.RecoilAnimation?.Play(0); // Play recoil animation from the beginning
                FireArm.anim.RecoilAimAnimation?.Play(0); // Play recoil aim animation from the beginning
            }
            
            // Apply camera recoil adjustments if the character manager is assigned
            if (FireArm.Owner)
            {
                // Calculate the recoil values based on the preset and firearm attachments, considering aiming state
                //TODO 부착물에 따른 MOD 적용
                //float vRecoil = verticalRecoil * firearmAttachmentsManager.recoil;
                float vRecoil = FireArm.fireArmData.verticalRecoil;
                float hRecoil = FireArm.fireArmData.horizontalRecoil;
                float camRecoil = FireArm.fireArmData.cameraRecoil;
            
                FireArm.Owner.AddLookValue(vRecoil, hRecoil);
            
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

        public Vector3 CalculatePattern(Vector3 forward, Vector3 right, Vector3 up)
        {
            float aimProgress = FireArm.anim.AimingAnimation.Progress;
            // 조준 애니메이션의 진행도를 기반으로 탄퍼짐 정도 선택
            float spray = (aimProgress >= 0.99) ? currentADSSpray : currentHipSpray;
            
            // 상하 랜덤
            float upDown = Random.Range(-1f, 1f);
            // 좌우 랜덤
            float rightLeft = Random.Range(-1f, 1f);
            
            Vector3 offset = (up * upDown + right * rightLeft);

            //TODO 이게 프리셋의 그 값인지 확인.
            offset.x *= horizontalMultiplier;
            offset.y *= verticalMultiplier;
            offset *= spray / 180f;

            return forward + offset * 2f;
        }

        public float GetCurrentSpray()
        {
            float aimProgress = FireArm.anim.AimingAnimation.Progress;
            return (aimProgress >= 0.99) ? currentADSSpray : currentHipSpray;
        }

    }
}