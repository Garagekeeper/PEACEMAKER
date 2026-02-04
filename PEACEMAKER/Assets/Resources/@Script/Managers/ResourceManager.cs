using Resources.Script.Creatures;
using UnityEngine;
using static Resources.Script.Defines;
namespace Resources.Script.Managers
{
    public class ResourceManager
    {
        private ObjCatalog _sourceCatalog;
        public ObjCatalog SourceCatalog { get => _sourceCatalog; private set => _sourceCatalog = value; }

        public ResourceManager(ObjCatalog  sourceCatalog)
        {
            _sourceCatalog = sourceCatalog;
        }
        
        public GameObject Instantiate(EObjectID key, Transform parent = null)
        {
            GameObject go = null;
            ObjectPreset preset = _sourceCatalog.GetObjPreset(key);
            if (preset == null)
            {
                Debug.LogError($"Failed to load preset : {key}");
                return null;
            }

            if (preset.poolable)
            {
                go = HeadManager.Pool.Pop(preset.prefab, parent);
                return go;
            }
            
            go = Object.Instantiate(preset.prefab, parent);
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