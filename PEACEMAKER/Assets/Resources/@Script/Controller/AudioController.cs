using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resources.Script.Audio;
using Resources.Script.Managers;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Resources.Script.Controller
{
    /// <summary>
    /// Audio의 제어를 담당하는 class
    /// </summary>
    public class AudioController
    {
        public AudioPreset Preset { get; private set; }
        private AudioSource _source;
        /// <summary>
        /// 0과 1사이의 랜덤한 값, pitch에 더해서 다양한 변주를 주기 위함.
        /// </summary>
        private float _randomizedPitchOffset;
        /// <summary>
        /// 6D(앞뒤좌우상하) 에 관련한 오프셋
        /// </summary>
        private float _sixDimensionsPitchOffset;
        private float _distanceFromListener;

        private float TimeScaleSyncedPitch
        {
            get
            {
                if (Preset.syncPitchWithTimeScale)
                {
                    // 게임의 속도를 제어
                    // 1이 기본 커지면 빨라지고, 작아지면 느려짐
                    return Time.timeScale;
                }

                return 1;
            }
        }
        
        /// <summary>
        /// 초기화 되었는지 확인용
        /// </summary>
        private bool _isInit;
        public float EventsDuration { get; set; }

        /// <summary>
        /// Audio Preset에서 만든 Audio 이벤트
        /// </summary>
        private List<CustomAudioEvent> CustomAudioEvents { get; set; } = new();

        private bool IsEventEnabled { get; set; }
        
        public void Init(AudioPreset preset,  AudioSource source)
        {
            CustomAudioEvents.Clear();
            Preset = preset;
            _source = source;

            if (!ValidCheck()) return;

            EventsDuration = Preset.audioLayersDuration;
            SetAudioSourceValue();
            SetListenerValue();

            foreach (var layer in Preset.audioLayers)
            {
                if (layer.audioClip == null)
                {
                    Debug.LogWarning("CustomAudioLayer has no AudioClip assigned.", _source.gameObject);
                    continue;
                }
                
                if (layer.time < 0)
                    Debug.LogError("[Audio] Audio Profile's sound layer time can't be less than zero. Resetting to 0.", _source.gameObject);

                var layerTime = Mathf.Clamp(layer.time, 0.001f, float.MaxValue);
                
                AddCustomEnvent(() =>
                {
                    if (!_source)
                        Debug.LogWarning("AudioSource is null. Cannot play audio clip.", _source.gameObject);
                    else
                        _source.PlayOneShot(layer.audioClip);
                }, layerTime);
                
            }
            _isInit = true;
        }
        
        
        private bool ValidCheck()
        {
            if (Preset && _source) return true;
            #if UnityEngine
            Debug.LogError("AudioController::Init: preset or audio source are null.");
            #endif
            return false;

        }

        private void SetAudioSourceValue()
        {
            _source.clip = Preset.audioClip;
            _source.outputAudioMixerGroup = Preset.output;
            _source.mute = Preset.mute;
            _source.bypassEffects = Preset.bypassEffects;
            _source.bypassListenerEffects = Preset.bypassListenerEffects;
            _source.bypassReverbZones = Preset.bypassReverbZones;
            _source.playOnAwake = Preset.playOnAwake;
            _source.loop = Preset.loop;
            _source.priority = Preset.priority;

            //TODO 뒤에 곱해지는 MOD값은 설정의 볼륨값 
            _source.volume = Preset.volume * AudioListener.volume;
            _source.pitch = Preset.pitch;
            _source.panStereo = Preset.stereoPan;
            _source.spatialBlend = Preset.spatialBlend;
            _source.reverbZoneMix = Preset.reverbZoneMix;
            _source.dopplerLevel = Preset.dopplerLevel;
            _source.spread = Preset.spread;
            _source.rolloffMode = AudioRolloffMode.Linear;
            _source.minDistance = Preset.minDistance;
            _source.maxDistance = Preset.maxDistance;
        }

        private void SetListenerValue()
        {
            // 비활성화 될 때 멈추는 클립이면 멈춤
            if (!_source.gameObject.activeSelf && Preset.stopOnDisabled)
                Stop();
            
            // random pitch 계산
            if (_source.gameObject.activeSelf && Preset.useRandomPitchOffset)
            {
                CalcRandomPitch();
                _source.pitch = ( Preset.pitch + _randomizedPitchOffset + _sixDimensionsPitchOffset ) * TimeScaleSyncedPitch;
            }

            if (SystemManager.Audio.MainListener != null) return;
            
            var distance = Vector3.Distance(_source.transform.position, SystemManager.Audio.MainListener.transform.position);
            var blendVal = distance / Preset.maxDistance;
            
            Vector3 direction = (SystemManager.Audio.MainListener.transform.position - _source.transform.position).normalized;
            
            //6방향 내적
            var forwardDot = Mathf.Max(0, Vector3.Dot(direction, Vector3.forward));
            var backwardDot = Mathf.Max(0, Vector3.Dot(direction, Vector3.back));
            var rightDot = Mathf.Max(0, Vector3.Dot(direction, Vector3.right));
            var leftDot = Mathf.Max(0, Vector3.Dot(direction, Vector3.left));
            var upDot = Mathf.Max(0, Vector3.Dot(direction, Vector3.up));
            var downDot = Mathf.Max(0, Vector3.Dot(direction, Vector3.down));
            
            // 내적 합치기
            var totoalDot = forwardDot + backwardDot + rightDot + leftDot + upDot + downDot;
            var dirValue = 0f;

            if (totoalDot > 0)
            {
                float temp = 0;
                temp += forwardDot * Preset.forwardFactor;
                temp += backwardDot * Preset.backwardFactor;
                temp += rightDot * Preset.rightFactor;
                temp += leftDot * Preset.leftFactor;
                temp += upDot * Preset.upFactor;
                temp += downDot * Preset.downFactor;
                
                dirValue = temp / totoalDot;
            }

            _sixDimensionsPitchOffset = Mathf.Lerp(0, dirValue, Preset._6DSoundCurve.Evaluate(blendVal));
        }

        private void Stop()
        {
            if (!ValidCheck()) return;
            if (!_source.gameObject.activeSelf) return;
            _source.Stop();
        }

        private void CalcRandomPitch()
        {
            if (!ValidCheck()) return;
            if (!_source.gameObject.activeSelf) return;
            _randomizedPitchOffset = Random.Range(0, Preset.randomPitchOffset);
        }

        private void AddCustomEnvent(UnityAction action, float time)
        {
            CustomAudioEvents.Add(new CustomAudioEvent(time, action));
        }

        /// <summary>
        /// return longest duration
        /// 가장 긴 시간을 반환
        /// </summary>
        /// <returns></returns>
        public float CalcCustomEventDuration()
        {
            if (CustomAudioEvents.Count == 0) return 0;
            return CustomAudioEvents.Max(e => e.time);
        }

        public async void Play(bool useOneShot = false, AudioClip clipOverride = null)
        {
            try
            {
                if (!_isInit) return;
                if (!Application.isPlaying) return;
                if (!ValidCheck()) return;
                
                if (Preset.spatialBlend > 0 && Preset.simulateAcousticLatency)
                {
                    var time = 0f;
                    var distanceFromListener = 1f;
                    // 거리를 기준으로 소리를 재생 (멀리있는 건 더 늦게)
                    // 사실 소규모 게임이라 거리가 그렇게 먼 경우는 없을 것
                    // 있어도 1f 선에서 정리하자
                    if (SystemManager.Audio.MainListener != null)
                        distanceFromListener = Mathf.Clamp(Vector3.Distance(
                            SystemManager.Audio.MainListener.transform.position,
                            _source.transform.position) / 343f, 0f, 1f);
                
                    // 소리가 도달하는 순간까지 기다림.
                    while (time < distanceFromListener)
                    {
                        time += Time.deltaTime;
                        if (!Application.isPlaying) return;
                        await Task.Yield();
                    }
                }
                
                EnableEvents();
                InvokeCustomEvents();

                if (useOneShot)
                {
                    AudioClip clipToPlay = Preset.audioClip;
                    if (clipOverride) clipToPlay = clipOverride;
                    if (clipToPlay) _source.PlayOneShot(clipToPlay);
                }
                else
                {
                    _source.Play();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        /// <summary>
        /// Pauses the currently playing audio.
        /// </summary>
        public void Pause()
        {
            if(!ValidCheck()) return;

            _source?.Pause();
        }
        
        /// <summary>
        /// Resumes the paused audio.
        /// </summary>
        public void Unpause()
        {
            if(!ValidCheck()) return;

            _source?.UnPause();
        }

        private void EnableEvents()
        {
            IsEventEnabled = true;
        }

        private void DisableEvents()
        {
            IsEventEnabled = false;
        }

        private async void InvokeCustomEvents()
        {
            try
            {
                if (!Application.isPlaying) return;
                if (!ValidCheck()) return;
                if (CustomAudioEvents.Count == 0) return;

                var time = -Time.deltaTime;
                var currTime = 0f;
                var prevTime = 0f;

                // 마지막 프레때문에 이렇게 검사
                while (time < EventsDuration + Time.deltaTime)
                {
                    time += Time.deltaTime;
                    currTime += Time.deltaTime;
                    if (!Application.isPlaying) return;
                
                    // 게임 속도에 따라서 pitch 조절
                    if (_source) _source.pitch = Time.timeScale * Preset.pitch;
                    foreach (var customAudioEvent in CustomAudioEvents)
                    {
                        // 현재 시간의 구간이 이벤트 발동 시간을 포함하면
                        if (!(currTime > customAudioEvent.time) || !(prevTime < customAudioEvent.time)) continue;
                        if (IsEventEnabled && _source.gameObject.activeSelf)
                            customAudioEvent.Invoke();
                    }
                    prevTime = currTime;
                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        
    }

    /// <summary>
    /// Represents a custom audio event that can be invoked at a specific time.
    /// </summary>
    public class CustomAudioEvent
    {
        public float time;
        public UnityAction UnityAction { get; private set; }

        public CustomAudioEvent(float time, UnityAction action)
        {
            this.time = time;
            this.UnityAction = action;
        }

        public void Invoke()
        {
            if (UnityAction == null)
            {
                Debug.LogWarning("CustomAudioEvent action is null. Skipping invocation.");
                return;
            }

            UnityAction.Invoke();
        }
    }
}