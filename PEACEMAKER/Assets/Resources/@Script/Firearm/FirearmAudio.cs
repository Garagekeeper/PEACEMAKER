using Resource.Script.Managers;
using Resources.Script.Controller;
using UnityEngine;

namespace Resources.Script.Firearm
{
    public class FirearmAudio : MonoBehaviour
    {
        public FirearmController  Firearm{get; private set;}
        public void Init(FirearmController firearm)
        {
            Firearm = firearm;
        }

        public void PlayShotFire()
        {
            SystemManager.Audio.PlaySFX(Firearm.fireArmData.fireSound, 1f, 0, false);
        }
    }
}