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
        
        public static GameManager Game => Instance.GameInternal;
        public static InputManager Input => Instance.InputInternal;
        public static AudioManager Audio => Instance.AudioInternal;
        public static UIManager UI => Instance.UIInternal;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            InputInternal = gameObject.AddComponent<InputManager>();
            GameInternal = gameObject.AddComponent<GameManager>();
            AudioInternal = gameObject.AddComponent<AudioManager>();
            UIInternal = gameObject.AddComponent<UIManager>();
            
            
            // 씬이 로드되면 호출될 함수 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UI?.OnSceneLoaded();  // ADD
        }
    }
}