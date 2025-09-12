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

        if (ManagerAudio.instance == null)
        {
            var prefab = Resources.Load<ManagerAudio>("ManagerAudio");
            if (prefab != null)
            {
                Object.Instantiate(prefab);
                Debug.Log("ManagerAudio dibuat otomatis dari Resources.");
            }
            else
            {
                Debug.LogError("Prefab ManagerAudio tidak ditemukan di Resources!");
            }
        }

        if (UpToDatabase.instance == null)
        {
            var prefab = Resources.Load<UpToDatabase>("UpToDatabase");
            if (prefab != null)
            {
                Object.Instantiate(prefab);
                Debug.Log("UpToDatabase dibuat otomatis dari Resources.");
            }
            else
            {
                Debug.LogError("Prefab UpToDatabase tidak ditemukan di Resources!");
            }
        }
    }
}
