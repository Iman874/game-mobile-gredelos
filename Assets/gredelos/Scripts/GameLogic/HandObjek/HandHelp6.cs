using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandHelp6 : MonoBehaviour, IHandAnim
{
    [Header("Gerak & Fade")]
    public float baseSpeed = 1f;
    public float acceleration = 2f;
    public float fadeDuration = 0.5f;
    [Range(0f, 1f)]
    public float fadeStartPercent = 0.8f;
    public float delayBetweenAnimations = 0.3f; // jeda antar animasi
    public float yOffset = 0f; // offset relatif pada Y

    [Header("Mode Arah")]
    public bool forwardDirection = true; 
    // true  = start -> destination
    // false = destination -> start

    public bool IsPlaying => currentAnim != null;

    [Header("Referensi Posisi")]
    public Transform startPositionObject;       // posisi awal / tetap
    public List<Transform> destinationObjects;  // daftar destinasi

    private SpriteRenderer sr;
    private Coroutine currentAnim;
    private bool stopRequested = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation()
    {
        StopAllAnimations(); // pastikan berhenti dulu

        if (sr != null)
        {
            var c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        if (startPositionObject == null || destinationObjects == null || destinationObjects.Count == 0)
        {
            Debug.LogWarning("Start atau daftar destinasi belum di-assign di HandHelp6.");
            return;
        }

        stopRequested = false;
        currentAnim = StartCoroutine(PlayCycleLoop());
    }

    private IEnumerator PlayCycleLoop()
    {
        while (!stopRequested)
        {
            foreach (var dest in destinationObjects)
            {
                if (stopRequested) break;
                if (dest == null) continue;

                Vector3 startPos = startPositionObject.position;
                Vector3 destPos = dest.position;
                destPos.y -= yOffset;

                if (forwardDirection)
                {
                    // animasi dari START -> DEST
                    yield return HandSwipe(startPos, destPos);
                }
                else
                {
                    // animasi dari DEST -> START
                    yield return HandSwipe(destPos, startPos);
                }

                // jeda antar animasi
                yield return new WaitForSeconds(delayBetweenAnimations);
            }
        }

        currentAnim = null;
    }

    private IEnumerator HandSwipe(Vector3 from, Vector3 to)
    {
        if (sr == null) yield break;

        transform.position = from;

        // fade in cepat supaya muncul
        yield return Fade(0f, 1f, 0.2f);

        // gerak ke target sambil fade out
        yield return MoveAndFadeOut(transform, from, to);

        // pastikan invisible di akhir
        var c = sr.color;
        c.a = 0f;
        sr.color = c;
    }

    private IEnumerator MoveAndFadeOut(Transform obj, Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float moved = 0f;
        float currentSpeed = baseSpeed;
        Vector3 dir = (to - from).normalized;
        obj.position = from;

        var c = sr.color;

        while (moved < distance && !stopRequested)
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

        c.a = 0f; sr.color = c;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        var c = sr.color;
        c.a = from; sr.color = c;

        while (t < duration && !stopRequested)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to; sr.color = c;
    }

    public void StopAllAnimations()
    {
        stopRequested = true;

        if (currentAnim != null)
        {
            StopCoroutine(currentAnim);
            currentAnim = null;
        }

        // reset transparansi
        if (sr != null)
        {
            var c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
    }
}
