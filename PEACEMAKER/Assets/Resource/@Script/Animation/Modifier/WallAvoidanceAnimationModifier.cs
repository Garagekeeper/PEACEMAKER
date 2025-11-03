
using UnityEngine;
using UnityEngine.Serialization;

namespace Resource.Script.Animation.Modifier
{
    /// <summary>
    /// 벽에 가까워질 때 애니메이션을 보정해주는 모디파이아
    /// </summary>
    public class WallAvoidanceAnimationModifier : ProceduralAnimationModifier
    {
        
        public float raycastRange = 1;
        /// <summary>
        /// 감지할 레이어. (기본: 모든 레이어)
        /// </summary>
        public LayerMask layerMask = ~0;
        private RaycastHit _hit;

        public Vector3 position;
        public Vector3 rotation;


        private Vector3 _posVel;
        private Vector3 _rotVel;

        private Vector3 _pos;
        private Vector3 _rot;

        private float _clippingFactor;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // 0. 메인 카메라가 없으면 종료
            if(!_camera) return;

            // 현재 위치에서 정면 방향으로 raycastRange길이의 ray를 쏜다.
            // layermask에 해당하는 레이어의 충돌정보를 _hit에 담는다.
            // 충돌한 것이 있으면 true, 없으면 false
            
            // 1.벽이 감지된 경우
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hit, raycastRange, layerMask))
            {
                // 1-1. 애니메이션의 진행도를 벽까지의 비율로 설정후 애니메이션 재생 활성화
                // 벽에 가까울수록 0에 근접
                TargetAnimation.Progress = _hit.distance / raycastRange;
                TargetAnimation.IsPlaying = true;

                // 1-2 벽과의 거리에 따라서 위치/회전 보정
                _pos = Vector3.Lerp(position, Vector3.zero, _hit.distance / raycastRange);
                _rot = Vector3.Lerp(rotation, Vector3.zero, _hit.distance / raycastRange);
            }
            // 2. 벽이 감지되지 않은 경우
            else
            {
                // 2-1. 진행도 초기화, 애니메이션 비활성화
                TargetAnimation.Progress = 0;
                TargetAnimation.IsPlaying = false;

                // 2-2. 초기 상태로 복귀
                _pos = Vector3.zero;
                _rot = Vector3.zero;
            }
            
            // 3. 현재 위치/회전에서 목표 pos,rot까지 부드럽게 보간
            TargetPosition = Vector3.SmoothDamp(TargetPosition, _pos, ref _posVel, TargetAnimation.length / GlobalSpeed);
            TargetRotation = Vector3.SmoothDamp(TargetRotation, _rot, ref _rotVel, TargetAnimation.length / GlobalSpeed); 
        }
    }
}