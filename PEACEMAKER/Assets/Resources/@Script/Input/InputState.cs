using UnityEngine;

namespace Resources.Script.Input
{
    // 인풋 상태를 가지고 여러곳에 사용 
    public sealed class InputState
    {
        public Vector2 Move { get; internal set; }
        public Vector2 Look { get; internal set; }

        public bool FirePressed { get; internal set; }
        public bool FireReleased { get; internal set; }
        public bool FireHeld { get; internal set; }

        public bool ReloadPressed { get; internal set; }
        public bool ReloadReleased { get; internal set; }

        public bool JumpPressed { get; internal set; }

        public bool PausePressed { get; internal set; }

        public bool SprintPressed { get; internal set; }
        public bool SprintReleased { get; internal set; }
        public bool SprintHeld { get; internal set; }

        public bool CrouchState { get; internal set; }
        public bool CrouchToggled { get; internal set; }

        public bool AimPressed { get; internal set; }
        public bool AimReleased { get; internal set; }
        public bool AimHeld { get; internal set; }

        public bool LeanLeftHeld { get; internal set; }
        public bool LeanRightHeld { get; internal set; }

        public int? InventoryPressed { get; internal set; }

        // 1프레임 입력만 유지되어야 하는 변수들 초기화
        public void ClearFrame()
        {
            FirePressed = false;
            FireReleased = false;

            ReloadPressed = false;
            ReloadReleased = false;

            JumpPressed = false;

            PausePressed = false;

            SprintPressed = false;
            SprintReleased = false;

            AimPressed = false;
            AimReleased = false;

            CrouchToggled = false;

            InventoryPressed = null;
        }
    }
}