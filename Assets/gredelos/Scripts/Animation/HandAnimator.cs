using UnityEngine;
using System.Collections;

public class HandAnimator : MonoBehaviour
{
    [Header("Animasi Settings")]
    public float durasiAnimasi = 3f;
    public float jedaAnimasi = 5f;
    public float fadeDuration = 0.2f; // durasi fade

    /// <summary>
    /// Memainkan animasi loop PointerAnimation + fade untuk hand tertentu
    /// </summary>
    public void PlayAnimationLoop(GameObject hand)
    {
        if (hand == null)
        {
            Debug.LogWarning("HandAnimator: Hand object null!");
            return;
        }

        var pointer = hand.GetComponent<PointerAnimation>();
        if (pointer == null)
        {
            Debug.LogWarning("Hand object tidak memiliki PointerAnimation: " + hand.name);
            return;
        }

        var sr = hand.GetComponent<SpriteRenderer>();
        var rt = hand.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogWarning("Hand object tidak memiliki RectTransform: " + hand.name);
            return;
        }

        StartCoroutine(LoopPointerAnimation(hand, pointer, rt, sr));
    }

    private IEnumerator LoopPointerAnimation(GameObject hand, PointerAnimation pointer, RectTransform rt, SpriteRenderer sr)
    {
        while (hand.activeInHierarchy)
        {
            hand.SetActive(true);

            // fade in
            if (sr != null)
                yield return FadeSprite(sr, 0f, 1f, fadeDuration);

            // jalankan pointer animation
            pointer.PlayAnimation(rt, durasiAnimasi);

            float elapsed = 0f;
            while (elapsed < durasiAnimasi)
            {
                if (!hand.activeInHierarchy) yield break;
                elapsed += Time.deltaTime;
                yield return null;
            }

            // fade out
            if (sr != null)
                yield return FadeSprite(sr, 1f, 0f, fadeDuration);

            // tunggu jeda sebelum animasi berikutnya
            float jedaElapsed = 0f;
            while (jedaElapsed < jedaAnimasi)
            {
                if (!hand.activeInHierarchy) yield break;
                jedaElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, float from, float to, float duration)
    {
        float t = 0f;
        Color c = sr.color;
        c.a = from;
        sr.color = c;

        while (t < duration)
        {
            if (!sr.gameObject.activeInHierarchy) yield break;

            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to;
        sr.color = c;
    }
}
