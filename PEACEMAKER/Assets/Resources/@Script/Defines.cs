namespace Resources.Script
{
    public static class Defines
    { 
        /*-------------------------
        *           Ammo
        -------------------------*/
        public enum EAmmoType
        {
            R556 = 0,
        }
        
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
        *    Cursor Lock
        -------------------------*/
        public static bool IsCursorLocked { get; set; } = true;
        
        public enum EUpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
        
        /*-------------------------
        *    Creature Type
        -------------------------*/
        public enum ECreatureType
        {
            Player=0,
            Enemy=1,
            Npc=2,
        }
        
        
        public enum EVector3Direction
        {
            Forward = 0,
            Back = 1,
            Right = 2,
            Left = 3,
            Up = 4,
            Down = 5
        }
    }
}