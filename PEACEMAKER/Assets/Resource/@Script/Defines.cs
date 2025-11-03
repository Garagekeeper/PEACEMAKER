namespace Resource.Script
{
    public static class Defines
    {
        public static float GlobalAnimationSpeed = 1.0f;
        public static int MaxAnimationFramerate = 120;
        public static float GlobalAnimationWeight = 1.0f;
        
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
    }
}