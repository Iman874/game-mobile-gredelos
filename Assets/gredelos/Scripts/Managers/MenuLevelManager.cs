using UnityEngine;
using System.Collections.Generic;

public class MenuLevelManager : MonoBehaviour
{
    public RectTransform contentPanel;
    public float itemSpacing = 100f;

    [Header("Level Prefabs (Fixed)")]
    public List<GameObject> levelPrefabs; // Prefab level tetap di scene

    private LevelDataController levelData;

    void Awake()
    {
        levelData = LevelDataController.I;
    }

    void Start()
    {
        ArrangeLevelItems();
    }

    public void ArrangeLevelItems()
    {
        float currentX = 0f;

        // Cari level terakhir yang unlocked
        int lastUnlockedLevel = 0;
        for (int i = 0; i < levelPrefabs.Count; i++)
        {
            if (levelData.IsLevelUnlocked(i + 1))
            {
                lastUnlockedLevel = i + 1;
            }
            else
            {
                break; // stop di level pertama yang belum terbuka
            }
        }

        for (int i = 0; i < levelPrefabs.Count; i++)
        {
            GameObject prefab = levelPrefabs[i];
            if (prefab == null) continue;

            // Hanya tampilkan level terakhir terbuka + 1 level berikutnya
            if (i + 1 > lastUnlockedLevel + 1)
            {
                prefab.SetActive(false);
                continue;
            }

            // Instantiate prefab agar bisa diatur di scene
            GameObject go = Instantiate(prefab, contentPanel);
            go.SetActive(true);

            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();

            float width = rect.sizeDelta.x > 0 ? rect.sizeDelta.x : 200f;

            // Atur posisi horizontal
            rect.anchoredPosition = new Vector2(currentX + width / 2, 0);
            currentX += width + itemSpacing;
        }

        // Sesuaikan ukuran content panel agar muat semua item
        contentPanel.sizeDelta = new Vector2(currentX, contentPanel.sizeDelta.y);
    }
}
