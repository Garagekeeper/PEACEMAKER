using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public UIManager UIInternal { get; private set; }
        public SettingManager SettingInternal { get; private set; }
        public LoadingManager LoadingInternal { get; private set; }
        public ObjectManager ObjManagerInternal { get; private set; }
        
        public static GameManager Game => Instance.GameInternal;
        public static InputManager Input => Instance.InputInternal;
        public static AudioManager Audio => Instance.AudioInternal;
        public static UIManager UI => Instance.UIInternal;
        public static SettingManager Setting => Instance.SettingInternal;
        public static LoadingManager Loading => Instance.LoadingInternal;
        public static ObjectManager ObjManager => Instance.ObjManagerInternal;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            InputInternal = gameObject.GetComponent<InputManager>();
            GameInternal = gameObject.GetComponent<GameManager>();
            AudioInternal = gameObject.GetComponent<AudioManager>();
            UIInternal = gameObject.GetComponent<UIManager>();
            SettingInternal =  gameObject.GetComponent<SettingManager>();
            LoadingInternal = gameObject.GetComponent<LoadingManager>();
            ObjManagerInternal = gameObject.GetComponent<ObjectManager>();
            
            // 씬이 로드되면 호출될 함수 등록
            SceneManager.sceneLoaded -= OnSceneLoadedMy;
            SceneManager.sceneLoaded += OnSceneLoadedMy;
            
            
        }

        private void Start()
        {
            SceneManager.LoadSceneAsync("Main Menu");
        }

        private void OnSceneLoadedMy(Scene scene, LoadSceneMode mode)
        {
            UI?.OnSceneLoaded();  // ADD
            Audio.ReSetting();
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedMy;
        }
    }
}