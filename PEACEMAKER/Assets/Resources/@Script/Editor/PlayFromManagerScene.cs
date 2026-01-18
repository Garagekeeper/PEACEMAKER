#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayFromManagerScene
{
    private const string ManagerScenePath = "Assets/Resources/@Scenes/Manager.unity";
    private const string PendingSceneKey = "MANAGER_PENDING_SCENE";

    static PlayFromManagerScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        
        // Play 버튼 눌렀을 때 
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 현재 활성 씬 경로 저장
            var activeScenePath = SceneManager.GetActiveScene().path;

            // 이미 매니저씬이면 종료
            if (activeScenePath == ManagerScenePath)
                return;

            // 수정된 씬 저장 여부 확인
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // 사용자가 저장 취소하면 플레이도 취소
                EditorApplication.isPlaying = false;
                return;
            }

            // 원래 씬 경로 저장
            EditorPrefs.SetString(PendingSceneKey, activeScenePath);

            // 매니저씬 열기
            EditorSceneManager.OpenScene(ManagerScenePath);
            
            Debug.Log($"Open {ManagerScenePath} for initializing");
        }
        
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (!EditorPrefs.HasKey(PendingSceneKey))
                return;

            string restorePath = EditorPrefs.GetString(PendingSceneKey);
            EditorPrefs.DeleteKey(PendingSceneKey);

            // 안전장치: Bootstrap이면 복구, 아니면 굳이 건드리지 않음
            if (SceneManager.GetActiveScene().path == ManagerScenePath)
                EditorSceneManager.OpenScene(restorePath, OpenSceneMode.Single);
            
            Debug.Log($"Open {restorePath} after initializing");
        }
    }
    
}
#endif

