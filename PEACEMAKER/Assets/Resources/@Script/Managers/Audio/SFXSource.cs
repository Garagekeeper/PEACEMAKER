using System.Collections;
using Resource.Script.Managers;
using UnityEngine;

namespace Resources.Script.Managers.Audio
{
    public class SFXSource : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Coroutine _coroutine;
        

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

        private IEnumerator CDisableAfterPlay()
        {
            yield return new WaitForSeconds(_audioSource.clip.length + 0.1f);
            SystemManager.Audio.ReturnSFXToPool(this);
        }
    }
}