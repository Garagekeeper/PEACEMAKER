namespace Resource.Script
{
    public static class Defines
    {
        /*-------------------------
         *          Anim
         ------------------------*/
        public static float GlobalAnimationSpeed = 1.0f;
        public static int MaxAnimationFramerate = 120;
        public static float GlobalAnimationWeight = 1.0f;
        
        /*-------------------------
         *    Mouse sensitivity
         ------------------------*/
        public static float SensitivityMultiplier { get; set; } = 1;
        public static float XSensitivityMultiplier { get; set; } = 1;
        public static float YSensitivityMultiplier { get; set; } = 1;
        
        /*-------------------------
        *    Cursor Locke
        ------------------------*/
        public static bool IsCursorLocked { get; set; } = true;
        
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
    }
}