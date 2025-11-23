using Resources.Script.Managers;
using Resources.Script.Managers.Audio;
using UnityEngine;
using static Resources.Script.Utilities;
namespace Resource.Script.Managers
{
    public class SystemManager : MonoBehaviour
    {
        private static SystemManager instance;
        public static SystemManager Instance
        {
            get
            {
                if (instance != null) return instance;
                var go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject("@Managers");
                    go.AddComponent<SystemManager>();
                }
                instance = go.GetComponent<SystemManager>();
                return instance;
            }
        }

        private InputManager _input;
        private GameManager _game;
        private AudioManager _audio;
        
        public static InputManager Input => Instance?._input;
        public static GameManager Game => Instance?._game;
        public static AudioManager Audio => Instance?._audio;

        private void Awake()
        {
            if (instance != null) return;
            
            GameObject go = GameObject.Find("@Manager");
            
            if (go != null) return;
            
            go = new GameObject("@SoundPool");
            DontDestroyOnLoad(go);
            
            go = new GameObject("@Manager");
            go.AddComponent<SystemManager>();
            DontDestroyOnLoad(go);
            
            instance = go.GetComponent<SystemManager>();
            _input = new InputManager();
            _game = new GameManager();
            _audio = instance.gameObject.AddComponent<AudioManager>();
        }

        private void Update()
        {
            _input?.OnUpdate();
        }

        private void LateUpdate()
        {
            _input?.LateUpdate();
        }
        
    }

    interface IManagerBase
    {
        public void OnUpdate();
    }
}
