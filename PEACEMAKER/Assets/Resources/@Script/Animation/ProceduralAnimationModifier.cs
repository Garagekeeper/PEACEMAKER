using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Animation
{
    [RequireComponent(typeof(ProceduralAnimation)), AddComponentMenu("")]
    public class ProceduralAnimationModifier : MonoBehaviour
    {
        /// <summary>
        /// The target animation that this modifer is modifying
        /// </summary>
        public ProceduralAnimation TargetAnimation { get; set; }

        /// <summary>
        /// final position result for this modifier
        /// ProceduralAnimation의 res 변수에서 처리
        /// </summary>
        public Vector3 TargetPosition { get; set; }

        /// <summary>
        /// final rotation result for this modifier
        /// ProceduralAnimation의 res 변수에서 처리
        /// </summary>
        public Vector3 TargetRotation { get; set; }

        // TODO MAKE GLOBAL VARIABLE CLASS
        /// <summary>
        /// 전역으로 관리하는 애니메이션 속도
        /// </summary>
        public float GlobalSpeed => GlobalAnimationSpeed;

        /// <summary>
        ///  전역으로 관리하는 애니메이션 프레임레이트
        /// </summary>
        public int MaxFramerate => MaxAnimationFramerate;
    }
}