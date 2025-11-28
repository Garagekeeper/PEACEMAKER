using System.Collections.Generic;
using Resources.Script.Audio;
using UnityEngine;

namespace Resources.Script.Managers
{
    public class AudioManager : MonoBehaviour,IManagerBase
    {
        
        private int _initialPoolSize;
        private Queue<SFXSource> _sfxPool;
        private GameObject _parent;
        private SFXSource _sfxPrefab;
        public AudioListener MainListener { get; private set; }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            // GameScene으로 넘어오면서 @SoundPool 오브젝트가 DontDestroyOnLoad에 생성됨
            _parent = GameObject.Find("@SoundPool");
            _initialPoolSize = 30;
            _sfxPool = new Queue<SFXSource>();
            _sfxPrefab = UnityEngine.Resources.Load<GameObject>("@Prefabs/sfx").GetComponent<SFXSource>();
            MainListener = FindAnyObjectByType<AudioListener>();
            InitPool();
        }

        public void InitPool()
        {
            if (_sfxPool.Count >= _initialPoolSize) return;

            // Pool에 미리 생성
            for (int i = 0; i < _initialPoolSize; i++)
                AddNewSFXSource();
        }

        public void AddNewSFXSource()
        {
            //오브젝트 인스턴스화 후에 큐에 집어넣음
            SFXSource sfxObj = Instantiate(_sfxPrefab, _parent.transform);
            
            ReturnSFXToPool(sfxObj);
        }

        public void ReturnSFXToPool(SFXSource sfx)
        {
            // 큐에 넣을때는 active false로
            if (!sfx) return;
            _sfxPool.Enqueue(sfx);
            sfx.gameObject.SetActive(false);
        }

        public SFXSource GetSFXFromPool()
        {
            // 큐에 남아있지 않으면 추가
            if (_sfxPool.Count == 0)
                AddNewSFXSource();

            // 큐에서 뽑아내서 반환
            var sfx = _sfxPool.Dequeue();
            sfx.gameObject.SetActive(true);

            return sfx;
        }

        // 오디오 클립 재생
        public SFXSource PlaySFX(AudioClip clip, float volume = 1f, float spatialBlend = 1f, bool isLoop = false)
        {
            var sfx = GetSFXFromPool();
            sfx.Play(clip, volume,spatialBlend, isLoop);
            return sfx;
        }

        public SFXSource PlayWithPreset(AudioPreset preset, Vector3 pos)
        {
            var sfx = GetSFXFromPool();
            sfx.transform.position = pos;
            sfx.PlayWithPreset(preset);
            return sfx;
        }

        // 게임 초기화등에 사용
        public void ReturnSFXToPoolAll()
        {
            foreach (Transform childTransform in _parent.transform)
            {
                if (_sfxPool.Count <= _initialPoolSize)
                    ReturnSFXToPool(childTransform.gameObject.GetComponent<SFXSource>());
                else 
                    GameObject.Destroy(childTransform.gameObject);
            }
        }
        public void OnUpdate()
        {
            
        }
    }

    class SFX
    {
        private GameObject _gameObject;
        private AudioClip _audioClip;

        public SFX(GameObject sfxObj)
        {
            
        }
    }
}