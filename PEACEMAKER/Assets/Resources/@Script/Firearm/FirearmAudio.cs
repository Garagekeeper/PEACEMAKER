using System;
using Resources.Script.Audio;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;

namespace Resources.Script.Firearm
{
    public class FirearmAudio : MonoBehaviour
    {
        public FirearmController  Firearm{get; private set;}
        private SFXSource _reloadSound;
        public void Init(FirearmController firearm)
        {
            Firearm = firearm;
        }

        public void PlayShotFire()
        {
            //SystemManager.Audio.PlaySFX(Firearm.fireArmData.fireSound, 1f, 0, false);
            SystemManager.Audio.PlayWithPreset(Firearm.preset.presetFireSound, Firearm.Owner.transform);
        }

        public void PlayReload()
        {
            _reloadSound = SystemManager.Audio.PlayWithPreset(Firearm.fireArmData.reloadSoundPreset,
                Firearm.Owner.transform);
        }

        private void OnDisable()
        {
            if (_reloadSound && _reloadSound.isActiveAndEnabled)
                _reloadSound.Mute();
        }
    }
}