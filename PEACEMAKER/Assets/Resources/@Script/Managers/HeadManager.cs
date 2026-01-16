using System;
using Resources.Script.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Script.Managers
{
    public class HeadManager : MonoBehaviour
    {
        public static bool Initialized { get; set; } = false;
        private static HeadManager sInstance;
        public static HeadManager Instance { get { Init(); return sInstance; } }

        public UIManager UIInternal { get; private set; }
        public SettingManager SettingInternal { get; private set; }
        
        
        private AudioManager _audio = new AudioManager();
        private GameManager _game = new GameManager();
        private InputManager _input;
        private PoolManager _pool = new PoolManager();
        private ObjectManager _obj = new ObjectManager();
        private ResourceManager _resource;
        
        public static AudioManager Audio => Instance?._audio;
        public static GameManager Game => Instance?._game;
        public static InputManager Input 
        { 
            get => Instance?._input;
            private set => Instance._input = value;
        }

        public static ResourceManager Resource
        { 
            get => Instance?._resource;
            private set => Instance._resource = value;
        }
        public static ObjectManager ObjManager => Instance._obj;
        public static PoolManager Pool =>Instance?._pool;

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


        public static UIManager UI => Instance.UIInternal;
        public static SettingManager Setting => Instance.SettingInternal;

        public static void Init()
        {
            if (sInstance == null && !Initialized)
            {
                Initialized = true;
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject("@Managers");
                    go.AddComponent<HeadManager>();
                }
                
                DontDestroyOnLoad(go);
                
                sInstance = go.GetComponent<HeadManager>();
            }
        }
        
        private void Awake()
        {
            if (sInstance != null && sInstance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);

            _audio.Init();
            var reader = GetComponent<InputReader>();
            if (reader == null) reader = gameObject.AddComponent<InputReader>();
            _input = new InputManager(reader);

            var catalog = GetComponent<ScriptableObjCatalog>();
            if (catalog == null) catalog = gameObject.AddComponent<ScriptableObjCatalog>();
            _resource = new ResourceManager(catalog);
            
            UIInternal = gameObject.GetComponent<UIManager>();
            SettingInternal =  gameObject.GetComponent<SettingManager>();
            Loading = gameObject.GetComponent<LoadingManager>();
            
            // 씬이 로드되면 호출될 함수 등록
            //SceneManager.sceneLoaded -= OnSceneLoadedMy;
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