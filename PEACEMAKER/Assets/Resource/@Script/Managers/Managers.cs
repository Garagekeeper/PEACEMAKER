using UnityEngine;

namespace Resource.Script.Managers
{
    public class Managers : MonoBehaviour
    {
        private static Managers instance;
        public static Managers Instance
        {
            get
            {
                if (instance != null) return instance;
                var go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject("@Managers");
                    go.AddComponent<Managers>();
                }
                instance = go.GetComponent<Managers>();
                return instance;
            }
        }

        private InputManager _input;
        private GameManager _game;
        
        public static InputManager Input => Instance?._input;
        public static GameManager Game => Instance?._game;

        private void Awake()
        {
            if (instance != null) return;
            
            _input = new InputManager();
            _game = new GameManager();
            
            GameObject go = GameObject.Find("@Manager");
            if (go != null) return;
            
            go = new GameObject("@Manager");
            go.AddComponent<Managers>();
            
            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();
        }

        private void Update()
        {
            _input?.OnUpdate();
        }
        
    }

    interface IManagerBase
    {
        public void OnUpdate();
    }
}
