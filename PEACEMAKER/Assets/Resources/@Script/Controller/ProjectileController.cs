using Resources.Script.Controller;
using UnityEngine;
using UnityEngine.Events;
using static Resources.Script.Utilities;
using static Resources.Script.Defines;

namespace Resource.Script.Controller
{
    public class ProjectileController : MonoBehaviour
    {
        [Header("Base Settings")]
        public LayerMask hittableLayers = -1;
        public EVector3Direction decalDirection = EVector3Direction.Forward;
        public float speed = 50;
        public float gravityMod = 1;
        public float force = 10;
        public int lifeTime = 5;
        public GameObject defaultDecal;
        public float hitRadius = 0.03f;

        [Header("Additional Settings")]
        public bool destroyOnImpact = false;
        public bool useSourceVelocity = true;
        public bool useAutoScaling = true;
        public float scaleMultipler = 45;

        [Header("Range Control")]
        public float range = 300;
        public AnimationCurve damageRangeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.3f) });


        public FirearmController source { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 initialVelocity { get; set; }
        public float maxVelocity { get; protected set; }
        private Vector3 velocity;
        private TrailRenderer trail;

        private Rigidbody rb;

        private Vector3 previousPosition;

        private Transform Effects;

        private Vector3 startPosition;

        public UnityEvent<GameObject, Ray, RaycastHit> onHit { get; set; } = new UnityEvent<GameObject, Ray, RaycastHit>();

        public GameObject sourcePlayer { get; set; }

        protected virtual void DestroySelf()
        {
            if (isActive)
                Destroy(gameObject, lifeTime);
        }

        /// <summary>
        /// Setup all this projectile's fields.
        /// </summary>
        /// <param name="source">Source firearm which this projectile will copy things from</param>
        /// <param name="direction">The direction of movement for this projectile</param>
        /// <param name="initialVelocity">The initial velocity for this projectile. By default this field is used for shooter velocity</param>
        /// <param name="speed">The maximum speed for this projectiles</param>
        /// <param name="range">The maximum distance from the initial firing location, if this is in half of the distance and max damage is 10, damage will be 5.</param>
        public void Setup(FirearmController source, Vector3 direction, Vector3 initialVelocity, float speed, float range)
        {
            //TODO 총구 부착물 따라서 수치 조정
            float muzzleModifier = 1;

            //Player and source firearm
            this.source = source;

            if (source && source.Owner != null)
                this.sourcePlayer = source.Owner.gameObject;

            //Direction and speed
            this.direction = direction;
            this.initialVelocity = initialVelocity;
            this.range = range;
            this.speed = speed * muzzleModifier;

            //Scale
            if (!source) return;
            useAutoScaling = source.preset.tracerRounds;
            scaleMultipler = source.preset.projectileSize;

            //Final setup
            source.Projectiles?.Add(this);
        }

        private void Awake()
        {
            previousPosition = transform.position;
            startPosition = transform.position;
        }

        protected virtual void Start()
        {
            trail = GetComponentInChildren<TrailRenderer>();
            rb = GetComponent<Rigidbody>();
            
            Vector3 sourceVelocity = useSourceVelocity ? initialVelocity : Vector3.zero;

            velocity = (direction) * (speed) + sourceVelocity;

            if (isActive)
                rb.AddForce(velocity, ForceMode.VelocityChange);

            if (source)
                maxVelocity = source.preset.muzzleVelocity;

            if (transform.Find("Effects"))
            {
                Effects = transform.Find("Effects");
                Effects.parent = null;
                Destroy(gameObject, lifeTime + 1);
            }

            transform.localScale = useAutoScaling ? Vector3.zero : Vector3.one * scaleMultipler;

            if (trail) trail.widthMultiplier = useAutoScaling ? 0 : scaleMultipler;

            if(isActive)
                Destroy(gameObject, lifeTime);
        }

        public float CalculateDamage()
        {
            // 처음 위치에서 여기까지의 거리
            float distanceFromStartPos = Vector3.Distance(transform.position, startPosition);

            // 한번 쏘는데 몇개의 총알을 쏘는지
            // 샷건의 경우 1번 쏘는데 총알 여러개
            float countFactor =  source.requiredAmmoType.BulletCountOnce;

            if (source)
            {
                distanceFromStartPos = Mathf.Clamp(distanceFromStartPos, 1, float.MaxValue);

                // 사거리가 1이라는 정도를 가질 때 현재 위치가 어떤 비율인지 확인
                float posToRange = distanceFromStartPos / range;

                
                posToRange = Mathf.Clamp01(posToRange);

                // 데미지 감소량을 계산 (커브에 x값을 넣어서 y값을 받아옴
                float damageToRange = damageRangeCurve.Evaluate(posToRange);

                //최종 데미지 계산
                float finalDamage = damageToRange * source.requiredAmmoType.BaseDamage / countFactor;

                return finalDamage;
            }

            Debug.LogError("Couldn't calculate damage due to null source firearm field. Damage will be default to 30.", gameObject);

            return 30;
        }

        private void Update()
        {
            // 이전 위치에서 현재 위치로 ray 생성
            Ray ray = new Ray(previousPosition, -(previousPosition - transform.position));
            float distance = Vector3.Distance(transform.position, previousPosition);

            // 구 형태로 충돌감지
            RaycastHit[] hits = Physics.SphereCastAll(ray, hitRadius, distance, hittableLayers);

            // 충돌한 오브젝트에 hit 처리
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                // 충돌지점의 좌표가 유효한 값이고, 총알이 이동한 후에 hit처리
                if (hit.point != Vector3.zero && distance != 0)
                {
                    UpdateHits(ray, hit);
                }
            }

            // 총알과 트레이서를 원근법에 맞게 조절
            if (useAutoScaling)
            {
                float scale = 1;
                var mainCamera = GetMainCamera();

                if (mainCamera)
                {
                    //메인 카메라부터의 거리 계산
                    var distanceFromMainCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
                    // 크기 배율 산출
                    scale = (distanceFromMainCamera * scaleMultipler) * (mainCamera.fieldOfView / 360);
                }
            
                //크기 조절
                transform.localScale = Vector3.one * scale;
                //트레이서의 크기도 조잘
                if (trail) trail.widthMultiplier = scale;
            }
            else
            {
                transform.localScale = Vector3.one * scaleMultipler;
            }

            if (Effects)
            {
                Effects.position = transform.position;
            }
        }

        private void FixedUpdate()
        {
            // 중력 적용
            rb.AddForce(Physics.gravity * gravityMod, ForceMode.Acceleration);
        }

        private void LateUpdate()
        {
            // 이전 위치 갱신
            // 물리 연산이 이뤄지는 프레임에서 계산
            previousPosition = transform.position;
        }

        protected virtual void UpdateHits(Ray ray, RaycastHit hit)
        {
            if (!source) return;

            if (!hit.transform) return;

            // TODO 충돌무시하는 오브젝트 관련
            //stop if object has ignored component
            //if (hit.transform.TryGetComponent(out Ignore _ignore) && _ignore.ignoreHitDetection || sourcePlayer && hit.transform == sourcePlayer.transform) return;
            // 네트워크용
            //onHit?.Invoke(hit.transform.gameObject, ray, hit);
            OnHit(hit);

            if (!isActive) return;

            source.shooter.UpdateHits(defaultDecal, ray, hit, CalculateDamage(), decalDirection);
        }

        public bool isActive { get; set; } = true;

        public virtual void OnHit(RaycastHit hit)
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, hitRadius);
        }

        [ContextMenu("Setup/Network Components")]
        public void Convert()
        {
            
        }
    }
}