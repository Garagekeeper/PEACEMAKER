using UnityEngine;
using Resources.Script.Managers;
using static Resources.Script.Defines;

namespace Resources.Script.InteractiveObject
{
    public class ExpGem : InteractiveObj, IPickup
    {
        private enum GemState { None, Dropping, Idle, Attracting }

        [Header("Drop Settings")]
        [SerializeField] private float dropDuration = 1.2f;   // 천천히 이동하도록 설정
        [SerializeField] private float jumpHeight = 0.8f;     // 톡 튀어오르는 높이
        [SerializeField] private float dropRange = 1.5f;      // 퍼지는 범위
        [SerializeField] private LayerMask groundMask;

        [Header("Rotate Settings")]
        [SerializeField] private bool xRotation;
        [SerializeField] private bool yRotation;
        [SerializeField] private bool zRotation;
        [SerializeField] private float rotationSpeed;
        
        
        [Header("Attract Settings")]
        [SerializeField] private float attractSpeed = 15f;

        private GemState _state = GemState.None;
        private Vector3 _startPos;
        private Vector3 _targetPos;
        private float _elapsedTime;
        private Transform _playerTransform;
        private Collider _collider;
        public ERarity Rarity { get; private set; }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _state = GemState.None;
            ObjectType = EObjectType.ExpGem;
        }

        /// <summary>
        /// 젬 드랍 초기화
        /// </summary>

        public void Init(Vector3 spawnPos, EObjectID rarity)
        {
            ERarity rarityEnum = ERarity.Normal;
            switch (rarity)
            {
                case EObjectID.ExpGemNormal:
                    rarityEnum = ERarity.Normal;
                    break;
                case EObjectID.ExpGemRare:
                    rarityEnum = ERarity.Rare;
                    break;
                case EObjectID.ExpGemEpic:
                    rarityEnum = ERarity.Epic;
                    break;
            }
            
            Init(spawnPos, rarityEnum);
        }
        public void Init(Vector3 spawnPos, ERarity rarity)
        {
            _startPos = spawnPos;
            _elapsedTime = 0f;

            // 1. 랜덤 목표 지점 설정
            Vector2 randomCircle = Random.insideUnitCircle * dropRange;
            Vector3 tempTarget = spawnPos + new Vector3(randomCircle.x, 0, randomCircle.y);
            groundMask = LayerMask.GetMask("Environment");

            // 2. 바닥 체크 및 정확한 높이 계산
            if (Physics.Raycast(spawnPos, Vector3.down, out var hit, 10.0f,groundMask))
            {
                // 최종 목적지
                _targetPos = CalculateGroundPosition(hit.point + new Vector3(tempTarget.x, 0, tempTarget.z));
            }
            else
            {
                // 바닥을 못 찾을 경우 현재 높이 기준 자동 계산
                _targetPos = CalculateGroundPosition(new Vector3(tempTarget.x, spawnPos.y, tempTarget.z));
            }

            Rarity = rarity;
            _state = GemState.Dropping;
        }

        /// <summary>
        /// 콜라이더 바운드를 이용해 모델이 바닥에 딱 붙는 위치를 계산
        /// </summary>
        private Vector3 CalculateGroundPosition(Vector3 hitPoint)
        {
            if (_collider == null) return hitPoint;
            
            // 콜라이더의 반지름
            float pivotToBottomOffset = _collider.bounds.center.y - _collider.bounds.min.y;
            
            // 바닥에서 콜라이더의 지름 만큼 떨어뜨려서 공중에 뜨도록
            return hitPoint + Vector3.up * (pivotToBottomOffset * 2);
        }

        private void Update()
        {
            if (_state == GemState.None) return;

            switch (_state)
            {
                case GemState.Dropping:
                    UpdateDropping();
                    break;
                case GemState.Idle:
                    UpdateRotate();
                    break;
                case GemState.Attracting:
                    UpdateAttracting();
                    break;
            }
        }

        private void UpdateDropping()
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / dropDuration);

            // 부드러운 시작과 끝을 위한 Easing (선택 사항)
            float smoothedT = Mathf.SmoothStep(0, 1, t);

            // 상하 운동
            Vector3 currentPos = Vector3.Lerp(_startPos, _targetPos, smoothedT);
            
            // 좌우 운동 (y = 4 * h * t * (1-t))
            float height = 4 * jumpHeight * t * (1 - t);
            currentPos.y += height;

            transform.position = currentPos;

            if (t >= 1f)
            {
                _state = GemState.Idle;
            }
        }

        public void SetAttract(bool attract)
        {
            _state = GemState.Attracting;
        }

        private void UpdateAttracting()
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                HeadManager.Game.MainPlayer.dropTransform.position, 
                attractSpeed * Time.deltaTime
            );
        }

        private void UpdateRotate()
        {
            Vector3 rotationVector = new Vector3(
                xRotation ? 1 : 0,
                yRotation ? 1 : 0,
                zRotation ? 1 : 0
            );
            transform.Rotate(rotationVector * (rotationSpeed * Time.deltaTime));
        }

        public void OnPickedUp()
        {
            HeadManager.ObjManager.Despawn(this);
        }
        
    }
}