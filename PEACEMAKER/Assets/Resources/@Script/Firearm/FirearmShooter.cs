using Resources.Script.Controller;
using Resources.Script.Creatures;
using Resources.Script.Decal;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Defines;
using static Resources.Script.Utilities;

namespace Resources.Script.Firearm
{
    public class FirearmShooter : MonoBehaviour
    {
        public FirearmController FireArm { get; private set; }
        protected LayerMask mask;
        private Camera _mainCam;

        /// <summary>
        /// Tracks the delay between shots fired.
        /// </summary>
        private float fireTimer;

        /// <summary>
        /// The number of shots fired in the current session.
        /// </summary>
        public int ShotsFired { get; protected set; }

        private Vector3 _currentFirePosition;
        private Quaternion _currentFireRotation;
        private Vector3 _currentFireDirection;
        private Vector3 _originalFireDirection;
        public float LastFireTime { get; set; } = -999f;

        public EFiringMode FiringMode { get; private set; }
        public EShotMechanism ShotMechanism { get; set; }

        public void Init(FirearmController controller)
        {
            FireArm = controller;
            FiringMode = FireArm.fireArmData.firingMode;
            ShotMechanism = FireArm.fireArmData.shotMechanism;

            _mainCam = GetMainCamera();
            //mask |= LayerMask.GetMask("FPS Object");
            mask |= LayerMask.GetMask("Environment");
            //mask |= LayerMask.GetMask("Iteractable");
            mask |= LayerMask.GetMask("Enemy");
        }

        public void UpdateFire()
        {
            Debug.DrawRay(_mainCam.transform.position, _mainCam.transform.forward * 30f, Color.red);
            if (FireArm.FirearmState == EFirearmStates.Fire)
                Fire();

        }

        private void Fire()
        {
            if (FireArm.anim.SprintingAnimation.Progress >= 0.1f) return;
            if (FireArm.FirePrevented) return;
            if (FireArm.AmmoInMagazine <= 0) return;
            var firePosition = Vector3.zero;
            var fireRotation = Quaternion.identity;
            var fireDirection = Vector3.zero;
            var muzzle = FireArm.muzzle;

            // Muzzle to CamForward
            // 총구에서 카메라 중앙으로 투사체 발사.
            // 정확한 에임
            //TODO 최대거리 지정
           //Debug.DrawRay(_mainCam.transform.position, _mainCam.transform.forward * 300f, Color.red);
            if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward,
                    out var hitInfo,300f, mask))
            {
                // 적정거리 이상으로 떨어져 있는 경우, 총구에서 시작해서 화면 정중앙으로 
                if (hitInfo.distance > 1f)
                {
                    firePosition = muzzle.position;
                    fireRotation = muzzle.rotation;
                    fireDirection = (hitInfo.point - muzzle.position).normalized;
                }
                else
                {
                    firePosition = _mainCam.transform.position;
                    fireRotation = _mainCam.transform.rotation;
                    fireDirection = _mainCam.transform.forward;
                }
            }
            else
            {
                firePosition = muzzle.position;
                fireRotation = muzzle.rotation;
                fireDirection = _mainCam.transform.forward;
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
                FireArm.recoilAndSpray.ApplyRecoil();
                _originalFireDirection = fireDirection;

                //TODO 여기서 반동 (Spray) 구현
                var finalDir = FireArm.recoilAndSpray.CalculatePattern(fireDirection, FireArm.muzzle.right, FireArm.muzzle.up);
                //SFX 적용
                //PlaySound
                FireArm.faudio.PlayShotFire();

                //Debug.Log($"{fireDirection}, {firnaldir}");
                FireDone(firePosition, fireRotation, finalDir);
            }

            fireTimer = Time.time + (60f / FireArm.fireArmData.fireRate);
        }

        /// <summary>
        /// 제한된 애니메이션을 플레이하고 있는지 확인, 줍기 뽑기 등의 행동중에는 재장전 사격 불가
        /// Checks if any of the animators are currently playing a restricted animation.
        /// </summary>
        /// <returns></returns>
        private bool IsPlayingRestrictedAnimation()
        {
            return FireArm.anim.animator.CurrentPlayingAnim("Take") ||
                   FireArm.anim.animator.CurrentPlayingAnim("Pickup");
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
            LastFireTime = Time.time;
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
            if (ShotMechanism == Defines.EShotMechanism.HitScan)
            {
                Ray ray = new Ray(FireArm.muzzle.position, direction);

                if (Physics.Raycast(ray, out RaycastHit hit, FireArm.fireArmData.range, mask))
                {
                    // Apply Damage, 데미지 적용
                    var playerMultiplier = FireArm.Owner.DamageMultiplier;
                    float calculatedDmg = FireArm.ammoItemInInventory.GetAmmoDmg();
                    float finalDmg = FireArm.alwaysApplyFire
                        ? calculatedDmg * playerMultiplier
                        : calculatedDmg * playerMultiplier / FireArm.ammoItemInInventory.ammo.BulletCountOnce;
                    UpdateHits(FireArm.fireArmData.defaultDecalPrefab, ray, hit, finalDmg, FireArm.DecalDirection);
                }
            }
            //1. Projctile
            else if (ShotMechanism == Defines.EShotMechanism.Projectile)
            {
            }

            FireArm.AmmoInMagazine = Mathf.Max(0, FireArm.AmmoInMagazine - 1);
            FireArm.FirearmState = Defines.EFirearmStates.None;
        }


        /// <summary>
        /// Updates the state of objects hit by a projectile, including applying damage, handling decals, and applying forces.
        /// </summary>
        /// <param name="defaultDecal">The default decal to apply on the hit surface.</param>
        /// <param name="ray">The ray that represents the projectile's path.</param>
        /// <param name="hit">Information about the hit result.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="decalDirection">The direction for orienting the decal.</param>
        public void UpdateHits(GameObject defaultDecal, Ray ray, RaycastHit hit, float damage,
            Defines.EVector3Direction decalDirection)
        {
            if (FireArm.OwnerController.CharacterController == null)
            {
                Debug.LogError("Character in the firearm is not set.");
                return;
            }

            var creature = FireArm.OwnerController.CharacterController.GetComponent<Creature>();

            // Invoke hit callbacks for the firearm
            InvokeHitCallbacks(FireArm.OwnerController.CharacterController.gameObject, ray, hit);

            // Exit if the hit target is the same as the character
            if (hit.transform == FireArm.OwnerController.CharacterController.transform) return;

            // 부착물 등 내부의 요소에 의해서 결정되는 배수
            float damageMultiplierInternal = 1;
            // 선택한 능력치에 따라서 결정되는 배수
            float damageMultiplierExternal = 1;

            // Handle damageable groups
            // 데미지를 받을 수 있는 부위
            bool shouldHighlight = false;
            if (hit.transform.TryGetComponent(out IDamageablePart damageablePart))
            {
                // 뒤에 1 부착물 mod
                damageMultiplierInternal = damageablePart.DamageMultiplier * 1f;
                shouldHighlight = damageablePart.DamageMultiplier != 1 ? true : false;
            }

            // 데미지를 받는 객체
            IDamageable damageable = hit.transform.FindSelfChildParent<IDamageable>();

            // Handle damageable objects
            if ( damageable is { IsDead: false })
            {
                var totalDamage = damage * damageMultiplierInternal * damageMultiplierExternal;

                //GameObject creatureGo = null;

                //if (creature) creatureGo = creature.gameObject;

                damageable.OnDamage(totalDamage, creature, hit.point ,shouldHighlight);
            }

            // 총알 자국 처리
            // Handle custom decals
            if (hit.transform.TryGetComponent(out CustomDecal customDecal))
            {
                defaultDecal = customDecal.decalVFX;
            }

            // Exit if the hit target is the same as the character manager
            if (FireArm.OwnerController?.transform == hit.transform) return;

            // Apply default or custom decal
            if (defaultDecal)
            {
                Vector3 hitPoint = hit.point;
                Quaternion decalRotation = GetHitRotation(hit);
                GameObject decalInstance = Instantiate(defaultDecal, hitPoint, decalRotation);

                decalInstance.transform.localScale *= FireArm.fireArmData.decalSize;
                decalInstance.transform.SetParent(hit.transform);

                float decalLifetime = customDecal?.lifeTime ?? 60f;
                Destroy(decalInstance, decalLifetime);
            }

            // 맞은 물체에 RB가 있으면 피격되서 흔들리도록 모션을 준다.
            // Apply force to the rigidbody if present
            // if (hit.rigidbody)
            // {
            //     var impact = FireArm.fireArmData.shotDelay <= 0f
            //         ? (FireArm.fireArmData.impactForce / FireArm.requiredAmmoType.BulletCountOnce)
            //         : FireArm.fireArmData.impactForce;
            //
            //     hit.rigidbody.AddForceAtPosition(-hit.normal * impact, hit.point, ForceMode.Impulse);
            // }
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
            // if (hit.transform.TryGetComponent<Ignore>(out Ignore _ignore) && _ignore.ignoreHitDetection) return;
            //
            // // Create a HitInfo object to store details about the hit
            // HitInfo hitInfo = new HitInfo(sourcePlayer, hit, ray.origin, ray.direction);
            //
            // // Retrieve the GameObject that was hit
            // GameObject obj = hit.transform.gameObject;
            //
            // // Try to get the IOnAnyHit interface implementation on the hit object, its children, and its parent
            // IOnAnyHit onAnyHit = obj.transform.GetComponent<IOnAnyHit>();
            // IOnAnyHitInChildren onAnyHitInChildren = obj.transform.GetComponentInParent<IOnAnyHitInChildren>();
            // IOnAnyHitInParent onAnyHitInParent = obj.transform.GetComponentInChildren<IOnAnyHitInParent>();
            //
            // // Try to get the IOnHit interface implementation on the hit object, its children, and its parent
            // IOnHit onHit = obj.transform.GetComponent<IOnHit>();
            // IOnHitInChildren onHitInChildren = obj.transform.GetComponentInParent<IOnHitInChildren>();
            // IOnHitInParent onHitInParent = obj.transform.GetComponentInChildren<IOnHitInParent>();
            //
            // // Invoke the OnHit method on the IOnHit interface, if implemented
            // onHit?.OnHit(hitInfo);
            //
            // // Invoke the OnHitInChildren method on the IOnHitInChildren interface, if implemented
            // onHitInChildren?.OnHitInChildren(hitInfo);
            //
            // // Invoke the OnHitInParent method on the IOnHitInParent interface, if implemented
            // onHitInParent?.OnHitInParent(hitInfo);
            //
            // // Invoke the OnAnyHit method on the IOnAnyHit interface, if implemented
            // onAnyHit?.OnAnyHit(hitInfo);
            //
            // // Invoke the OnAnyHitInChildren method on the IOnAnyHitInChildren interface, if implemented
            // onAnyHitInChildren?.OnAnyHitInChildren(hitInfo);
            //
            // // Invoke the OnAnyHitInParent method on the IOnAnyHitInParent interface, if implemented
            // onAnyHitInParent?.OnAnyHitInParent(hitInfo);
        }
    }
}