using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Resources.Script.Managers
{
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject radialImage;
        [SerializeField] private GameObject radialText;
        private Image _radialProgressImg;
        private TextMeshProUGUI _radialProgressText;
        
        private void Awake()
        {
            _radialProgressImg = radialImage.GetComponent<Image>();
            _radialProgressText = radialText.GetComponent<TextMeshProUGUI>();   
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            loadingScreen.SetActive(true);
            Time.timeScale = 0f;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            float visualProgress = 0f;
            while (!asyncLoad.isDone)
            {
                float target = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                // 부드럽게 따라가게
                visualProgress = Mathf.MoveTowards(
                    visualProgress,
                    target,
                    Time.unscaledDeltaTime * 0.5f   // 속도 조절
                );
                _radialProgressText.text = (visualProgress * 100).ToString($"F0") + "%";

                _radialProgressImg.fillAmount = visualProgress;

                if (asyncLoad.progress >= 0.9f && visualProgress >= 0.99f)
                {
                    _radialProgressImg.fillAmount = 1f;
                    _radialProgressText.text = "100%";
                    yield return new WaitForSecondsRealtime(0.5f);
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
            loadingScreen.SetActive(false);
            _radialProgressImg.fillAmount = 0;
            _radialProgressText.text = "0 %";
            Time.timeScale = 1f;
        }
    }
}