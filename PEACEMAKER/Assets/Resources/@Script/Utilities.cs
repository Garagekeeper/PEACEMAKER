using System.Collections.Generic;
using Resources.Script.Managers;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script
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
            if (HeadManager.Game.IsPaused) return;
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
        
        /// <summary>
        /// projectile Decal
        /// 총알 자국을 지정된 방향으로 변환하는 함수
        /// </summary>
        /// <param name="raycastHit"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Quaternion GetFromToRotation(RaycastHit raycastHit, EVector3Direction direction)
        {
            Quaternion result = new Quaternion();

            switch (direction)
            {
                case EVector3Direction.Forward:
                    result = Quaternion.FromToRotation(Vector3.forward, raycastHit.normal);
                    break;

                case EVector3Direction.Back:
                    result = Quaternion.FromToRotation(Vector3.back, raycastHit.normal);
                    break;

                case EVector3Direction.Right:
                    result = Quaternion.FromToRotation(Vector3.right, raycastHit.normal);
                    break;

                case EVector3Direction.Left:
                    result = Quaternion.FromToRotation(Vector3.left, raycastHit.normal);
                    break;

                case EVector3Direction.Up:
                    result = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
                    break;

                case EVector3Direction.Down:
                    result = Quaternion.FromToRotation(Vector3.down, raycastHit.normal);
                    break;
            }

            return result;
        }

        /// <summary>
        /// hitscan Decal
        /// </summary>
        /// <param name="raycastHit"></param>
        /// <returns></returns>
        public static Quaternion GetHitRotation(RaycastHit raycastHit)
        {
            Vector3 normal = raycastHit.normal;
            // normal이 위/아래 방향과 너무 일치하면 보정
            Vector3 up = Mathf.Abs(normal.y) > 0.99f ? Vector3.forward : Vector3.up;

            return Quaternion.LookRotation(normal, up);
        }
        
        
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rnd = Random.Range(0, i + 1);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }

        /// <summary>
        /// 연산자 타입에 따라서 계산
        /// </summary>
        /// <param name="a"></param>
        /// <param name="op"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ChangeValue(float a, EOperator op, float b)
        {
            return op switch
            {
                EOperator.Add => a + b,
                EOperator.Sub => a - b,
                EOperator.Mul => a * b,
                EOperator.Div => Mathf.Approximately(b, 0f) ? a : a / b,
                _ => a
            };
        }
    };
}