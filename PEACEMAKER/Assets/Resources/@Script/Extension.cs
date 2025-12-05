using Resources.Script.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resources.Script
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
        
        
        
        /// <summary>
        /// Tries to find a component of type <typeparamref name="T"/> on the current GameObject,
        /// then its children, and then its parents.
        /// Returns the first component found or null if none is found.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="component">The Component whose GameObject hierarchy to search.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects when searching children and parents.</param>
        /// <returns>The first found component of type T or null if none found.</returns>
        public static T FindSelfChildParent<T>(this Component component, bool includeInactive = false, bool skipParent = false)
        {
            if (!component) return default(T);

            var comp = component.GetComponent<T>();

            if (comp != null) return comp;

            var childComp = component.GetComponentInChildren<T>(includeInactive);

            if (childComp != null) return childComp;

            var parentComp = component.GetComponentInParent<T>(includeInactive);

            if(parentComp != null && !skipParent) return parentComp;

            return comp;
        }

        public static void TurnOnOffFirearms(this FirearmHUD firearmHUD, bool state)
        {
            firearmHUD.gameObject.SetActive(state);
        }
    }
}