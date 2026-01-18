using System.Collections.Generic;
using Resources.Script.Audio;
using static Resources.Script.Defines;
using UnityEngine;

namespace Resources.Script.Managers
{
    public class AudioManager
    {
        
        private int _initialPoolSize;
        private GameObject _parent;
        public AudioListener MainListener { get; private set; }

        public void Init()
        {
            MainListener = Object.FindAnyObjectByType<AudioListener>();
        }

        public void ReSetting()
        {
            MainListener = Object.FindAnyObjectByType<AudioListener>();
        }
        
        // 오디오 클립 재생
        public SFXSource PlaySFX(AudioClip clip, float volume = 1f, float spatialBlend = 1f, bool isLoop = false)
        {
            var sfx = HeadManager.Resource.Instantiate(EObjectID.SFX, HeadManager.ObjManager.SoundRoot).GetComponent<SFXSource>();
            sfx.Play(clip, volume,spatialBlend, isLoop);
            return sfx;
        }
        
        public SFXSource PlayWithPreset(AudioPreset preset, bool posNeeded = false)
        {
             var sfx = HeadManager.Resource.Instantiate(EObjectID.SFX, HeadManager.ObjManager.SoundRoot).GetComponent<SFXSource>();
             if (HeadManager.Game.MainPlayer && posNeeded)
                 sfx.transform.position = HeadManager.Game.MainPlayer.PController.transform.position;
             sfx.PlayWithPreset(preset);
            // var sfx = GetSFXFromPool();
            // if (HeadManager.Game.MainPlayer && posNeeded)
            //     sfx.transform.position = HeadManager.Game.MainPlayer.PController.transform.position;
            // sfx.PlayWithPreset(preset);
            return sfx;
        }
        
        public SFXSource PlayWithPreset(AudioPreset preset, Transform target)
        {
            var sfx = HeadManager.Resource.Instantiate(EObjectID.SFX, HeadManager.ObjManager.SoundRoot).GetComponent<SFXSource>();
            sfx.PlayWithPreset(preset, target);
            return sfx;
        }
    }
}