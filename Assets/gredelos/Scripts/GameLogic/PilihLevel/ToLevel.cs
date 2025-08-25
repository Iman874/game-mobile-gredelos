using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ToLevel : MonoBehaviour
{
    public string levelSceneName; // langsung drag scene di Inspector
    private static LevelDataController levelData; // akses statis

    void Awake()
    {
        levelData = LevelDataController.I;
    }

    public void MulaiLevel(int level)
    {
        // Cari scene berdasarkan nama level
        if (levelSceneName != null)
        {

            // Ubah is_main = true jika main level 1
            if (levelData != null)
            {
                levelData.SetIsMainLevel(level);
            }

            SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.LogError("Scene belum diatur!");
        }
    }
}
