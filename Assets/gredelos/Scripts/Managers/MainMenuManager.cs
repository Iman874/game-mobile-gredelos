using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset; // drag .unity file di Inspector
#endif

    [SerializeField, HideInInspector] private string sceneName;

    public void OnStartButtonPressed()
    {
        Debug.Log("Scene name: " + sceneName);

        // Pastikan LevelDataController sudah ada
        if (LevelDataController.I == null)
        {
            Debug.LogError("LevelDataController belum ada! Harus ada di bootstrap scene.");
            return;
        }

        // Mulai login
        LevelDataController.I.StartLogin(LevelDataController.I.GetPlayerName());

        // Buat data level jika belum ada apapun di game_data.json
        if (!LevelDataController.I.HasLevelData())
        {
            LevelDataController.I.LoadDataLevel();
        }
        else
        {
            Debug.LogWarning("LevelDataController sudah memiliki data level.");
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene belum ditentukan.");
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
