using UnityEngine;

public class CekHadiahProgress : MonoBehaviour
{
    public static LevelDataController levelData;

    void Awake()
    {
        levelData = LevelDataController.I;
    }

    public void CekHadiah(int nomorGameplay)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelDataController tidak ditemukan!");
            return;
        }

        if (nomorGameplay == 1)
        {
            // Cek hadiah untuk gameplay 1
        }
        
    }
}
