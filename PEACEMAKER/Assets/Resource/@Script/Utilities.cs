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
    };
}