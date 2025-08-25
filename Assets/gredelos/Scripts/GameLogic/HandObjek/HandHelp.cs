using UnityEngine;
using System.Collections;

public class HandHelp : MonoBehaviour
{
    public float baseSpeed = 1f;
    public float acceleration = 2f;
    public float swipeDistance = 10f;
    public float fadeDuration = 0.5f;
    public float fadeStartPercent = 0.8f;
    public int repeatCount = 2;

    [Header("Arah Animasi")]
    public bool animRight = true; // default true biar tidak rusak yang lama
    public bool animLeft = false; // kalau dicentang, jalanin kiri

    private Vector3 startPos;
    private Vector3 endPos;     // kanan
    private Vector3 endPosLeft; // kiri
    private SpriteRenderer sr;
    private Coroutine currentAnim;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        endPos = startPos + new Vector3(swipeDistance, 0, 0);
        endPosLeft = startPos - new Vector3(swipeDistance, 0, 0);
    }

    // Fungsi utama yang dipanggil banyak script
    public void PlayAnimation()
    {
        if (currentAnim != null)
            StopCoroutine(currentAnim);

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

        currentAnim = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // Kalau kanan dicentang → mainin animasi kanan dulu
        if (animRight)
            yield return StartCoroutine(HandSwipeAnimation(startPos, endPos));

        // Kalau kiri dicentang → mainin animasi kiri setelah kanan selesai
        if (animLeft)
            yield return StartCoroutine(HandSwipeAnimation(startPos, endPosLeft));

        currentAnim = null;
    }

    IEnumerator HandSwipeAnimation(Vector3 from, Vector3 to)
    {
        if (repeatCount != 1) // hanya untuk repeat selain 1 (rawan bug)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                // Awal: invisible
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
                transform.position = from;

                // Fade in
                yield return Fade(0f, 1f, fadeDuration);

                // Geser + Fade out
                yield return MoveAndFadeOut(transform, from, to, fadeDuration);
            }
        }
        else // kalau repeat 1, langsung jalanin tanpa loop (untuk menghindari bug)
        {
            // Awal: invisible
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            transform.position = from;

            // Fade in
            yield return Fade(0f, 1f, fadeDuration);

            // Geser + Fade out
            yield return MoveAndFadeOut(transform, from, to, fadeDuration);
        }
       
    }

    IEnumerator MoveAndFadeOut(Transform obj, Vector3 from, Vector3 to, float fadeTime)
    {
        float distance = Vector3.Distance(from, to);
        float moved = 0f;
        float currentSpeed = baseSpeed;

        Vector3 dir = (to - from).normalized;
        obj.position = from;

        Color c = sr.color;

        while (moved < distance)
        {
            obj.position += dir * currentSpeed * Time.deltaTime;
            moved += currentSpeed * Time.deltaTime;
            currentSpeed += acceleration * Time.deltaTime;

            float progress = moved / distance;

            if (progress >= fadeStartPercent)
            {
                float localProgress = (progress - fadeStartPercent) / (1f - fadeStartPercent);
                c.a = Mathf.Lerp(1f, 0f, localProgress);
                sr.color = c;
            }

            yield return null;
        }

        // Pastikan invisible
        c.a = 0f;
        sr.color = c;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = sr.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to;
        sr.color = c;
    }
}
