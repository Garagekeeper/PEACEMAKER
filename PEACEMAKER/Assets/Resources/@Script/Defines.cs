using System.Collections.Generic;

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

        public static List<string> AmmoNameList = new List<string>() { "5.56MM" };
        
        /*-------------------------
        *           Gun
        -------------------------*/
        public enum EFiringMode
        {
            SemiAuto = 0,
            Auto = 1,
            Burst = 2,
        }

        public enum EShotMechanism
        {
            Projectile = 0,
            HitScan = 1,
        }
        
        public enum EFirearmStates
        {
            None = 0,
            Reloading = 1,
            Fire = 2,
        };

        public enum EFirearmAimStates
        {
            LowReady = 0,
            ShoulderReady,
        };
        
        public enum EReloadType
        {
            Default = 0,
            /// <summary>
            /// 재장전 시 모든 동작이 코드에 의해서 통제됨
            /// 주로 장전 중에 발사가 가능한 샷건 종류에서 사용
            /// All of actions are controlled by script
            /// useful for shotgun(Fire while reloading)
            /// </summary>
            Scripted = 1
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