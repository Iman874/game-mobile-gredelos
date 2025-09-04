// HandHelp2.cs
using UnityEngine;
using System.Collections;

public class HandHelp2 : MonoBehaviour, IHandAnim
{
    [Header("Gerak & Fade")]
    public float baseSpeed = 1f;
    public float acceleration = 2f;
    public float swipeDistance = 10f;   // jarak dari tengah ke kanan/kiri
    public float fadeDuration = 0.5f;   // durasi fade-in
    [Range(0f, 1f)]
    public float fadeStartPercent = 0.8f; // mulai fade-out di % perjalanan ke tengah

    private Vector3 centerPos;  // target tengah
    private Vector3 rightPos;   // start kanan
    private Vector3 leftPos;    // start kiri
    private SpriteRenderer sr;
    private Coroutine currentAnim;
    private bool isPlaying;
    public bool IsPlaying => isPlaying;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        centerPos = transform.position;
        rightPos  = centerPos + new Vector3(swipeDistance, 0f, 0f);
        leftPos   = centerPos - new Vector3(swipeDistance, 0f, 0f);
    }

    public void PlayAnimation()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);

        // pastikan invisible dulu
        if (sr != null)
        {
            var c = sr.color; c.a = 0f; sr.color = c;
        }

        currentAnim = StartCoroutine(OneCycle());
    }

    private IEnumerator OneCycle()
    {
        // 1) dari kanan -> tengah
        yield return HandSwipe(rightPos, centerPos);

        // 2) dari kiri -> tengah
        yield return HandSwipe(leftPos, centerPos);

        currentAnim = null;
    }

    private IEnumerator HandSwipe(Vector3 from, Vector3 to)
    {
        if (sr == null) yield break;

        // set posisi & alpha 0
        transform.position = from;
        var c = sr.color; c.a = 0f; sr.color = c;

        // fade-in dulu
        yield return Fade(0f, 1f, fadeDuration);

        // gerak menuju tengah sambil fade-out (mulai di fadeStartPercent)
        yield return MoveAndFadeOut(transform, from, to);
        
        // pastikan invisible di akhir
        c = sr.color; c.a = 0f; sr.color = c;
    }

    private IEnumerator MoveAndFadeOut(Transform obj, Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float moved = 0f;
        float currentSpeed = baseSpeed;

        Vector3 dir = (to - from).normalized;
        obj.position = from;

        var c = sr.color;

        while (moved < distance)
        {
            float dt = Time.deltaTime;
            obj.position += dir * currentSpeed * dt;
            moved += currentSpeed * dt;
            currentSpeed += acceleration * dt;

            float progress = Mathf.Clamp01(moved / distance);

            if (progress >= fadeStartPercent)
            {
                float local = (progress - fadeStartPercent) / (1f - fadeStartPercent);
                c.a = Mathf.Lerp(1f, 0f, local);
                sr.color = c;
            }

            yield return null;
        }

        // jaga-jaga
        c.a = 0f; sr.color = c;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        var c = sr.color;
        c.a = from; sr.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to; sr.color = c;
    }
}
