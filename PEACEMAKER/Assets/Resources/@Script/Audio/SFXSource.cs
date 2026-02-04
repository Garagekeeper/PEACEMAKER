using System;
using System.Collections;
using Resources.Script.Controller;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Resources.Script.Audio
{
    public class SFXSource : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Coroutine _coroutine;
        public AudioController PAudioController { get; } = new();
        public Transform Target { get; set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
            // timescale이 0이어도 소리가 나오게 하려면
            _audioSource.ignoreListenerPause = true; // (선택사항) 리스너 자체가 일시정지인 경우
        }

        private void Update()
        {
            if (Target)
            {
                transform.position =  Target.position;
            }
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
            if (spatialBlend>0.1f)
                _audioSource.Play();
            else 
                _audioSource.PlayOneShot(clip);

            if (!isLoop)
                _coroutine = StartCoroutine(CDisableAfterPlay());
        }

        public void PlayWithPreset(AudioPreset preset, Transform target = null, bool oneshot = true)
        {
            StopAllCoroutines();
            Target = target;
            PAudioController.Init(preset, _audioSource);
            var length = PAudioController.CalcCustomEventDuration();
            PAudioController.Play(oneshot);
            if (!preset.loop)
                StartCoroutine(CDisableAfterPlayPrefab(length));
        }

        private IEnumerator CDisableAfterPlay()
        {
            yield return new WaitForSecondsRealtime(_audioSource.clip.length + 0.1f);
            HeadManager.Resource.Destroy(gameObject);
        }
        
        private IEnumerator CDisableAfterPlayPrefab(float time)
        {
            float offset = 0.1f;
            if (time != 0)
                offset = 1f;

            if (time == 0)
                time = PAudioController.Preset.audioClip.length;
            
            yield return new WaitForSecondsRealtime(time + offset);
            Target = null;
            HeadManager.Resource.Destroy(gameObject);
        }

        public void Mute()
        {
            _audioSource.volume = 0f;
        }
    }
}