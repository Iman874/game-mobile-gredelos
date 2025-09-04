using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandHelp5 : MonoBehaviour, IHandAnim
{
    [Header("Gerak & Fade")]
    public float baseSpeed = 1f;
    public float acceleration = 2f;
    public float fadeDuration = 0.5f;
    [Range(0f, 1f)]
    public float fadeStartPercent = 0.8f;
    public float delayBetweenAnimations = 0.3f;
    public float yOffset = 1.5f;

    [Header("Animasi Samping")]
    public float leftOffset = 0.5f;
    public float rightOffset = 0.5f;

    [Header("Referensi Posisi")]
    public Transform startPositionObject;
    public List<Transform> destinationObjects;

    private SpriteRenderer sr;
    private Coroutine currentAnim;

    public bool IsPlaying => currentAnim != null;

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
            Debug.LogWarning("Start atau daftar destinasi belum di-assign di HandHelp5.");
            return;
        }

        currentAnim = StartCoroutine(PlayCycleLoop());
    }

    private IEnumerator PlayCycleLoop()
    {
        foreach (var dest in destinationObjects)
        {
            if (dest == null || !dest.gameObject.activeInHierarchy)
                continue;

            Vector3 target = dest.position;
            target.y -= yOffset;

            // Animasi dari start → destinasi
            yield return HandSwipe(startPositionObject.position, target);

            // Animasi ke kiri / kanan sebelum fade out
            yield return SideSwipe(target);

            // Delay antar animasi
            yield return new WaitForSeconds(delayBetweenAnimations);

            if (!gameObject.activeInHierarchy) yield break;
        }

        currentAnim = null;
    }

    private IEnumerator HandSwipe(Vector3 from, Vector3 to)
    {
        if (sr == null) yield break;

        transform.position = from;

        // Fade in cepat
        yield return Fade(0f, 1f, 0.2f);

        // Gerak ke target
        yield return Move(transform, from, to);

        // Pastikan visible sebelum side swipe
        var c = sr.color;
        c.a = 1f;
        sr.color = c;
    }

    private IEnumerator SideSwipe(Vector3 origin)
    {
        // Pilih urutan acak: kiri dulu atau kanan dulu
        bool leftFirst = Random.value > 0.5f;

        Vector3 leftPos = origin + Vector3.left * leftOffset;
        Vector3 rightPos = origin + Vector3.right * rightOffset;

        if (leftFirst)
        {
            yield return Move(transform, origin, leftPos);
            yield return Move(transform, leftPos, rightPos);
        }
        else
        {
            yield return Move(transform, origin, rightPos);
            yield return Move(transform, rightPos, leftPos);
        }

        // Kembali ke posisi origin
        yield return Move(transform, transform.position, origin);

        // Fade out setelah side swipe
        yield return Fade(1f, 0f, fadeDuration);
    }

    private IEnumerator Move(Transform obj, Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float moved = 0f;
        float speed = baseSpeed;
        Vector3 dir = (to - from).normalized;
        obj.position = from;

        while (moved < distance)
        {
            if (!gameObject.activeInHierarchy) yield break;

            float dt = Time.deltaTime;
            obj.position += dir * speed * dt;
            moved += speed * dt;
            speed += acceleration * dt;

            yield return null;
        }

        obj.position = to;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        var c = sr.color;
        c.a = from;
        sr.color = c;

        while (t < duration)
        {
            if (!gameObject.activeInHierarchy) yield break;

            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to;
        sr.color = c;
    }
}
