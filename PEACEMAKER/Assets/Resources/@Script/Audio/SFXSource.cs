using System.Collections;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Resources.Script.Audio
{
    public class SFXSource : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Coroutine _coroutine;
        public AudioController PAudioController { get; } = new();

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
        }

        public void Play(AudioClip clip, float volume = 1f, float spatialBlend = 1f, bool isLoop = false)
        {
            StopAllCoroutines();
            float delay = Random.Range(0f, 0.02f);
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.loop = isLoop;
            _audioSource.spatialBlend = spatialBlend;
            //double offset = Random.Range(0f, 0.3f); // ±10ms 흔들기
            //_audioSource.PlayScheduled(AudioSettings.dspTime  + offset);
            _audioSource.Play();

            if (!isLoop)
                _coroutine = StartCoroutine(CDisableAfterPlay());
        }

        public void PlayWithPreset(AudioPreset preset, bool oneshot = true)
        {
            StopAllCoroutines();
            PAudioController.Init(preset, _audioSource);
            var length = PAudioController.CalcCustomEventDuration();
            PAudioController.Play(oneshot);
            if (!preset.loop)
                StartCoroutine(CDisableAfterPlayPrefab(length));
        }

        private IEnumerator CDisableAfterPlay()
        {
            yield return new WaitForSeconds(_audioSource.clip.length + 0.1f);
            SystemManager.Audio.ReturnSFXToPool(this);
        }
        
        private IEnumerator CDisableAfterPlayPrefab(float time)
        {
            float offset = 0.1f;
            if (time != 0)
                offset = 1f;

            if (time == 0)
                time = PAudioController.Preset.audioClip.length;
            
            yield return new WaitForSeconds(time + offset);
            SystemManager.Audio.ReturnSFXToPool(this);
        }
    }
}