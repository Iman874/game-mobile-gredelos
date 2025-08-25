using UnityEngine;
using System.Collections.Generic;

public class MenuLevelManager : MonoBehaviour
{
    public RectTransform contentPanel;
    public float itemSpacing = 100f;

    [Header("Level Prefabs (Fixed)")]
    public List<GameObject> levelPrefabs; // Prefab level tetap di scene

    void Start()
    {
        ArrangeLevelItems();
    }

    public void ArrangeLevelItems()
    {
        float currentX = 0f;

        for (int i = 0; i < levelPrefabs.Count; i++)
        {
            GameObject go = levelPrefabs[i];
            if (go == null) continue;

            // Pastikan parent ke contentPanel
            go.transform.SetParent(contentPanel, false);
            go.SetActive(true);

            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();

            float width = rect.sizeDelta.x > 0 ? rect.sizeDelta.x : 200f;

            // Atur posisi horizontal dengan spacing
            rect.anchoredPosition = new Vector2(currentX + width / 2, 0);
            currentX += width + itemSpacing;
        }

        // Sesuaikan ukuran content panel agar muat semua item
        contentPanel.sizeDelta = new Vector2(currentX, contentPanel.sizeDelta.y);
    }
}
