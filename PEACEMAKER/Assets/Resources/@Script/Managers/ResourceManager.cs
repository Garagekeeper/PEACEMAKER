using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Managers
{
    public class ResourceManager
    {
        private ScriptableObjCatalog _sourceCatalog;
        public ScriptableObjCatalog SourceCatalog { get; private set; }

        public ResourceManager(ScriptableObjCatalog  sourceCatalog)
        {
            _sourceCatalog = sourceCatalog;
        }
        
        public GameObject Instantiate(EObjectID key, Transform parent = null)
        {
            ObjectPreset preset = _sourceCatalog.GetObjPreset(key);
            if (preset == null)
            {
                Debug.LogError($"Failed to load preset : {key}");
                return null;
            }

            if (preset.poolable)
                return HeadManager.Pool.Pop(preset.prefab, parent);
            
            GameObject go = Object.Instantiate(preset.prefab, parent);
            //TODO 여기서 프리셋 기본값 저장
            go.name = preset.prefab.name;

            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null) return;
            if (HeadManager.Pool.Push(go)) return;
            Object.Destroy(go);
        }
    }
}