using System;
using Resources.Script.Input;
using Resources.Script.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Script.Managers
{
    public class HeadManager : MonoBehaviour
    {
        public static bool Initialized { get; set; } = false;
        private static HeadManager sInstance;

        public static HeadManager Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = FindFirstObjectByType<HeadManager>();
                if (sInstance == null)
                {
                    Debug.LogError("[HeadManager] Not found. Make sure Manager scene loads first and contains @Managers with HeadManager.");
                }
                return sInstance;
            }
        }

        public SettingManager SettingInternal { get; private set; }


        private AbilityManager _abilityPool;
        private AudioManager _audio = new AudioManager();
        private GameManager _game = new GameManager();
        private InputManager _input;
        private PoolManager _pool = new PoolManager();
        private ObjectManager _obj = new ObjectManager();
        private ResourceManager _resource;
        private UIManager _ui = new  UIManager();
        private SettingManager _setting = new  SettingManager();

        public static AbilityManager Ability => Instance?._abilityPool;
        public static AudioManager Audio => Instance?._audio;
        public static GameManager Game => Instance?._game;

        public static InputManager Input
        {
            get => Instance?._input;
            private set => Instance._input = value;
        }
        public static ObjectManager ObjManager => Instance._obj;
        public static ResourceManager Resource
        {
            get => Instance?._resource;
            private set => Instance._resource = value;
        }
        public static UIManager UI => Instance?._ui;

        public static PoolManager Pool => Instance?._pool;
        
        public static SettingManager Setting => Instance?._setting;

        #region MONO

        private LoadingManager _loading;

        public static LoadingManager Loading
        {
            get => Instance?._loading;
            private set
            {
                if (Instance != null)
                    Instance._loading = value;
            }
        }

        #endregion



        private void Awake()
        {
            Debug.Log("[HeadManager] Awake, Initialize Started");
            if (sInstance != null && sInstance != this)
            {
                Destroy(gameObject);
                return;
            }

            sInstance = this;
            DontDestroyOnLoad(gameObject);

            _audio.Init();
            _game.Init();
            _setting.Init();

            var reader = GetComponent<InputReader>() ?? gameObject.AddComponent<InputReader>();
            _input = new InputManager(reader);

            var catalog = GetComponent<ObjCatalog>() ?? gameObject.AddComponent<ObjCatalog>();
            _resource = new ResourceManager(catalog);
            
            var abilityPool = GetComponent<AbilityPool>() ?? gameObject.AddComponent<AbilityPool>();
            _abilityPool = new AbilityManager(abilityPool);
            
            Loading = GetComponent<LoadingManager>();

            //SceneManager.sceneLoaded -= OnSceneLoadedMy;
            //SceneManager.sceneLoaded += OnSceneLoadedMy;
        }
        


        private void Start()
        {
            // 에디터에서는 어떤 씬에서 시작해도 
            // 매니저 씬을 통과한 다음에 이동
#if UNITY_EDITOR
            const string key = "MANAGER_PENDING_SCENE";
            if (UnityEditor.EditorPrefs.HasKey(key))
            {
                string scenePath = UnityEditor.EditorPrefs.GetString(key);

                // path -> scene name 추출
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (SceneManager.GetActiveScene().name != sceneName)
                    SceneManager.LoadSceneAsync(sceneName);
                return;
            }
#endif

            // 빌드/일반 실행: 기본 시작 씬
            if (SceneManager.GetActiveScene().name != "Main Menu")
                SceneManager.LoadSceneAsync("Main Menu");
        }

        private void OnSceneLoadedMy(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            //UIInternal?.OnSceneLoaded(); // ADD
            _audio.ReSetting();
            //TODO
        }
        
        //
        // private void OnDisable()
        // {
        //     SceneManager.sceneLoaded -= OnSceneLoadedMy;
        // }
    }
}