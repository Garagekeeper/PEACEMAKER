using System;
using Resources.Script.Audio;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;

namespace Resources.Script.Firearm
{
    public class FirearmAudio : MonoBehaviour
    {
        public FirearmController  FirearmController{get; private set;}
        private SFXSource _reloadSound;
        public void Init(FirearmController firearmController)
        {
            FirearmController = firearmController;
        }

        public void PlayShotFire()
        {
            //SystemManager.Audio.PlaySFX(Firearm.fireArmData.fireSound, 1f, 0, false);
            HeadManager.Audio.PlayWithPreset(FirearmController.preset.presetFireSound);
        }

        public void PlayReload()
        {
            _reloadSound = HeadManager.Audio.PlayWithPreset(FirearmController.fireArmData.reloadSoundPreset,
                FirearmController.OwnerController.transform);
        }

        private void OnDisable()
        {
            if (_reloadSound && _reloadSound.isActiveAndEnabled)
                _reloadSound.Mute();
        }
    }
}