using UnityEngine;
using System.Collections;

public class PointerAnimation : MonoBehaviour
{
    private Vector2 startPos;
    private Vector3 startScale;

    [Header("Bob Movement")]
    public float offsetY = 15f; // seberapa naik
    public float speed = 2f;    // kecepatan naik-turun

    [Header("Click Scale")]
    public float scaleAmount = 0.1f;      // seberapa kecil skala
    public float scaleDuration = 0.15f;   // durasi mengecil

    private bool isScaling = false;

    public void PlayAnimation(RectTransform target, float duration)
    {
        startPos = target.anchoredPosition;
        startScale = target.localScale;

        StopAllCoroutines();
        StartCoroutine(Animate(target, duration));
    }

    private IEnumerator Animate(RectTransform target, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // === Bob ke atas-bawah ===
            float sinValue = Mathf.Sin(elapsed * speed * Mathf.PI); // pakai PI supaya lebih natural
            float y = startPos.y + sinValue * offsetY;
            target.anchoredPosition = new Vector2(startPos.x, y);

            // === Trigger klik saat di puncak gerakan ===
            if (sinValue > 0.95f && !isScaling)
            {
                StartCoroutine(ClickScale(target));
            }

            yield return null;
        }

        // reset ke kondisi awal
        target.anchoredPosition = startPos;
        target.localScale = startScale;
    }

    private IEnumerator ClickScale(RectTransform target)
    {
        isScaling = true;

        Vector3 smallScale = startScale * (1f - scaleAmount);
        float t = 0f;

        // Mengecil
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(startScale, smallScale, t / scaleDuration);
            yield return null;
        }

        // Kembali ke normal
        t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(smallScale, startScale, t / scaleDuration);
            yield return null;
        }

        target.localScale = startScale;
        isScaling = false;
    }
}
