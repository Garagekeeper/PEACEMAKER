using UnityEngine;

namespace Resources.Script.Managers
{
    public class SystemManager : MonoBehaviour
    {
        private static SystemManager instance;
        public static SystemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("@Managers");
                    instance = go.AddComponent<SystemManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public InputManager InputInternal { get; private set; }
        public GameManager GameInternal { get; private set; }
        public AudioManager AudioInternal { get; private set; }
        
        public static GameManager Game => Instance.GameInternal;
        public static InputManager Input => Instance.InputInternal;
        public static AudioManager Audio => Instance.AudioInternal;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            var go = new GameObject("@SoundPool"); DontDestroyOnLoad(go);

            InputInternal = new InputManager();
            GameInternal = new GameManager();
            AudioInternal = gameObject.AddComponent<AudioManager>();
        }

        private void Update()
        {
            Input?.OnUpdate();
        }

        private void LateUpdate()
        {
            Input?.LateUpdate();
        }
    }

    interface IManagerBase
    {
        void OnUpdate();
    }
}