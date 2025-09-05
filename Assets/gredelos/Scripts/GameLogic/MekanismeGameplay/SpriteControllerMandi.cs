using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteControllerMandi : MonoBehaviour
{
    [Header("Level Info")]
    public int nomorLevel = 2;
    public int nomorGameplay = 1; 

    [Header("Daftar Sprite")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Pengaturan")]
    public bool loopSprites = false;

    [Header("Efek Opsional")]
    public List<GameObject> busaEfekList = new List<GameObject>();

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites.Count > 0)
        {
            spriteRenderer.sprite = sprites[0];
            currentIndex = 0;
            UpdateBusaOpacity();
        }
    }

    public int ChangeSpriteMandi()
    {
        if (sprites.Count == 0) return 0;

        currentIndex++;

        if (currentIndex >= sprites.Count - 1)
        {
            if (loopSprites)
                currentIndex = 0;
            else
                currentIndex = sprites.Count - 1;

            CekSelesaiProgress();
        }

        spriteRenderer.sprite = sprites[currentIndex];
        UpdateBusaOpacity();

        Debug.Log($"Sprite berubah ke index {currentIndex}");

        int sisa = (sprites.Count - 1) - currentIndex;
        return Mathf.Max(0, sisa);
    }

    private void UpdateBusaOpacity()
    {
        if (sprites.Count <= 1) return;

        // Hitung progress: dari 1 (awal) ke 0 (akhir)
        float progress = 1f - (float)currentIndex / (sprites.Count - 1);

        foreach (var efek in busaEfekList)
        {
            if (efek != null)
            {
                var sr = efek.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = progress; // set alpha sesuai progress
                    sr.color = c;
                }
            }
        }
    }

    public void ResetSprite()
    {
        if (sprites.Count > 0)
        {
            currentIndex = 0;
            spriteRenderer.sprite = sprites[0];
            UpdateBusaOpacity();
        }
    }

    private void CekSelesaiProgress()
    {
        var controller = FindObjectOfType<ControllerPlayObjekLevel2>();
        if (controller == null)
        {
            Debug.LogWarning("ControllerPlayObjekLevel2 tidak ditemukan di scene!");
            return;
        }

        if (nomorLevel == 2)
        {
            controller.OnSelesaiProgress(nomorGameplay, nomorLevel);
            Debug.Log($"Progress {nomorGameplay} selesai di Level {nomorLevel}");
        }
    }
}
