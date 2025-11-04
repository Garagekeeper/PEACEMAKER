using UnityEngine;
using UnityEngine.InputSystem;

namespace Resource.Script
{
    public static class Extension
    {
        /// <summary>
        /// Checks for douple clicks and sets targetValue to true if the user has douple clicked
        /// </summary>
        /// <param name="inputAction"></param>
        /// <param name="targetValue"></param>
        /// <param name="lastClickTime"></param>
        /// <param name="maxClickTime"></param>
        /// <returns></returns>
        public static void HasDoubleClicked(this InputAction inputAction, ref bool targetValue, ref float lastClickTime, float maxClickTime = 0.5f)
        {
            if (inputAction.triggered)
            {
                float timeSinceLastSprintClick = Time.time - lastClickTime;

                if (timeSinceLastSprintClick < maxClickTime)
                {
                    targetValue = true;
                }

                lastClickTime = Time.time;
            }

            if (inputAction.IsPressed() == false) targetValue = false;
        }
        
        public static void SetRotation(this Transform transform, Quaternion rotation)
        {
            transform.localRotation = rotation;
        }
        
        // public static void SetRotation(this Transform transform, Quaternion rotation, bool isLocal = false)
        // {
        //      if (local) transform.localRotation = rotation;
        //
        //      else transform.rotation = rotation;
        // }
    }
}