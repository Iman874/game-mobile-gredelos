using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SplashScreenController : MonoBehaviour
{
    [Header("Durasi")]
    public float fadeInDuration = 1f;
    public float displayDuration = 2f;
    public float fadeOutDuration = 1f;

    [Header("Target UI")]
    public CanvasGroup fadeCanvasGroup;

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [SerializeField, HideInInspector] private string sceneName;

    void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            StartCoroutine(FadeSequence());
        }
        else
        {
            Debug.LogWarning("CanvasGroup belum di-assign!");
            Invoke(nameof(LoadMainMenu), fadeInDuration + displayDuration + fadeOutDuration);
        }
    }

    System.Collections.IEnumerator FadeSequence()
    {
        // Fade In
        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration));

        // Wait (gambar terlihat)
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeOutDuration));

        // Load scene berikutnya
        LoadMainMenu();
    }

    System.Collections.IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = to;
    }

    void LoadMainMenu()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene belum dipilih!");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(sceneAsset);
            sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
#endif
}
