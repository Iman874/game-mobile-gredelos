using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandHelp4 : MonoBehaviour, IHandAnim
{
    [Header("Gerak & Fade")]
    public float baseSpeed = 1f;
    public float acceleration = 2f;
    public float fadeDuration = 0.5f;
    [Range(0f, 1f)]
    public float fadeStartPercent = 0.8f;
    public float delayBetweenAnimations = 0.3f; // jeda antar animasi
    public float yOffset = 1.5f; // offset relatif pada Y

    public bool IsPlaying => currentAnim != null;


    [Header("Referensi Posisi")]
    public Transform startPositionObject;       // posisi tujuan tetap
    public List<Transform> destinationObjects;  // daftar destinasi

    private SpriteRenderer sr;
    private Coroutine currentAnim;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);

        if (sr != null)
        {
            var c = sr.color; c.a = 0f; sr.color = c;
        }

        if (startPositionObject == null || destinationObjects == null || destinationObjects.Count == 0)
        {
            Debug.LogWarning("Start atau daftar destinasi belum di-assign di HandHelp4.");
            return;
        }

        currentAnim = StartCoroutine(PlayCycleLoop());
    }

    private IEnumerator PlayCycleLoop()
    {
        foreach (var dest in destinationObjects)
        {
             if (dest == null)
            {
                Debug.Log("Destinasi null, skip");
                continue;
            }

            if (!dest.gameObject.activeInHierarchy)
            {
                Debug.Log($"Destinasi {dest.name} tidak aktif, skip");
                continue;
            }

            Debug.Log($"Animasi ke {dest.name}");
            // atur posisi awal dengan Y relatif
            Vector3 from = dest.position;
            from.y -= yOffset; // offset relatif

            // jalankan animasi dari DESTINASI → START (startPositionObject tetap)
            yield return HandSwipe(from, startPositionObject.position);

            // jeda antar animasi
            yield return new WaitForSeconds(delayBetweenAnimations);

            if (!gameObject.activeInHierarchy) yield break;
        }

        currentAnim = null;
    }

    private IEnumerator HandSwipe(Vector3 from, Vector3 to)
    {
        if (sr == null) yield break;

        transform.position = from;

        // fade in cepat supaya muncul
        yield return Fade(0f, 1f, 0.2f);

        // gerak ke target (startPositionObject) sambil fade out
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

        while (moved < distance)
        {
            if (!gameObject.activeInHierarchy) yield break;

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

        while (t < duration)
        {
            if (!gameObject.activeInHierarchy) yield break;

            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to; sr.color = c;
    }
}
