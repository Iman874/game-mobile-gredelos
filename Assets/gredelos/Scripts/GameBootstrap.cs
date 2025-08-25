using UnityEngine;

public static class GameBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (LevelDataController.I == null)
        {
            var prefab = Resources.Load<LevelDataController>("LevelDataController");
            if (prefab != null)
            {
                Object.Instantiate(prefab);
                Debug.Log("LevelDataController dibuat otomatis dari Resources.");
            }
            else
            {
                Debug.LogError("Prefab LevelDataController tidak ditemukan di Resources!");
            }
        }
    }
}
