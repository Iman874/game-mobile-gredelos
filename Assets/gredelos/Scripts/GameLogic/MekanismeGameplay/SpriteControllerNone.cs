using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteControllerNone : MonoBehaviour
{
    [Header("Daftar Sprite")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Pengaturan")]
    public bool loopSprites = false; // kalau true, setelah sprite terakhir balik ke sprite pertama

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set sprite pertama kalau ada
        if (sprites.Count > 0)
        {
            spriteRenderer.sprite = sprites[0];
            currentIndex = 0;
        }
    }

    /// <summary>
    /// Ganti sprite ke berikutnya.
    /// Return jumlah sprite tersisa sebelum habis.
    /// </summary>
    public int ChangeSprite()
    {
        if (sprites.Count == 0) return 0;

        currentIndex++;

        if (currentIndex >= sprites.Count)
        {
            if (loopSprites)
                currentIndex = 0; // balik ke awal
            else
                currentIndex = sprites.Count - 1; // berhenti di terakhir
        }

        spriteRenderer.sprite = sprites[currentIndex];
        Debug.Log($"Sprite berubah ke index {currentIndex}");

        int sisa = (sprites.Count - 1) - currentIndex;
        return Mathf.Max(0, sisa);
    }

    /// <summary>
    /// Reset sprite ke awal.
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
