using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteController : MonoBehaviour
{
    [Header("Level Info")]
    public int nomorLevel = 5;
    public int nomorGameplay = 121;

    [Header("Daftar Sprite")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Pengaturan")]
    public bool loopSprites = true; // kalau true, setelah sprite terakhir balik ke sprite pertama

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // set sprite pertama kalau ada
        if (sprites.Count > 0)
        {
            spriteRenderer.sprite = sprites[0];
            currentIndex = 0;
        }
    }

    /// <summary>
    /// Ubah sprite ke index berikutnya, return jumlah sprite tersisa sebelum habis
    /// </summary>
    public int ChangeSprite()
    {
        if (sprites.Count == 0) return 0;

        currentIndex++;

        if (currentIndex >= sprites.Count - 1)
        {
            if (loopSprites)
                currentIndex = 0; // balik ke awal
            else
                currentIndex = sprites.Count - 1; // berhenti di terakhir

            // --- Trigger event khusus ---
            if (nomorLevel == 5 && nomorGameplay == 121)
            {
                Debug.Log("Gameplay 121 selesai");

                var controller = FindObjectOfType<ControllerPlayObjekLevel5>();
                if (controller != null)
                {
                    controller.OnSelesaiProgress(121, 5);
                }
                else
                {
                    Debug.LogWarning("ControllerPlayObjekLevel5 tidak ditemukan di scene!");
                }
            }
            else if (nomorLevel == 5 && nomorGameplay == 122)
            {
                Debug.Log("Gameplay 122 selesai");

                var controller = FindObjectOfType<ControllerPlayObjekLevel5>();
                if (controller != null)
                {
                    controller.OnSelesaiProgress(122, 5);
                }
                else
                {
                    Debug.LogWarning("ControllerPlayObjekLevel5 tidak ditemukan di scane!");
                }
            }
            else if (nomorLevel == 5 && nomorGameplay == 123)
            {
                Debug.Log("Gameplay 123 selesai");

                var controller = FindObjectOfType<ControllerPlayObjekLevel5>();
                if (controller != null)
                {
                    controller.OnSelesaiProgress(123, 5);
                }
                else
                {
                    Debug.LogWarning("ControllerPlayObjekLevel5 tidak ditemukan di scane!");
                }
            }
        }

        spriteRenderer.sprite = sprites[currentIndex];
        Debug.Log($"Sprite berubah ke index {currentIndex}");

        // Hitung berapa sprite tersisa sebelum habis
        int sisa = (sprites.Count - 1) - currentIndex;
        if (sisa < 0) sisa = 0;

        return sisa;
    }

    /// <summary>
    /// Opsional: reset ke sprite pertama
    /// </summary>
    public void ResetSprite()
    {
        if (sprites.Count > 0)
        {
            currentIndex = 0;
            spriteRenderer.sprite = sprites[0];
        }
    }
}
