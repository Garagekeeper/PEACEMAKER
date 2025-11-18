using UnityEngine;
using static Resource.Script.Defines;

namespace Resource.Script
{
    public static class Utilities
    {
        /// <summary>
        /// vector의 모든 요소가 사용할 수 있는 값인지 반환
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static bool IsVector3Valid(Vector3 vector)
        {
            return !(float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z));
        }

        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsCursorLocked = true;
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IsCursorLocked = false;
        }

        public static Camera GetMainCamera()
        {
            return Camera.main != null ? Camera.main : Object.FindFirstObjectByType<Camera>();
        }

        /// <summary>
        /// stateName의 애니메이션을플레이 하고 있는지 여부 반환.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public static bool CurrentPlayingAnim(this Animator animator, string stateName)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    };
}