using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class PilihKarakterController : MonoBehaviour
{

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset; // drag .unity file di Inspector
#endif

    [SerializeField, HideInInspector] private string sceneName;
    public void PilihLakiLaki()
    {
        PlayerPrefs.SetInt("Gender", 0); // 0 = laki-laki
        // set juga untuk database
        LevelDataController.I.db.player[0].jenis_kelamin = "laki-laki";
        LevelDataController.I.Save();
        SceneManager.LoadScene(sceneName);

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene belum ditentukan.");
        }
    }

    public void PilihPerempuan()
    {
        PlayerPrefs.SetInt("Gender", 1); // 1 = perempuan
        // set juga untuk database
        LevelDataController.I.db.player[0].jenis_kelamin = "perempuan";
        LevelDataController.I.Save();
        SceneManager.LoadScene(sceneName);

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
