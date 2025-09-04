using UnityEngine;
using System.Collections;

public class HandHelp3 : MonoBehaviour, IHandAnim
{
    [Header("Gerak & Fade")]
    public float baseSpeed = 1f;
    public float acceleration = 2f;
    public float fadeDuration = 0.5f;   
    [Range(0f,1f)]
    public float fadeStartPercent = 0.8f;

    [Header("Referensi Posisi")]
    public Transform startPositionObject; // posisi awal
    public Transform endPositionObject;   // posisi akhir (hanya referensi)

    private SpriteRenderer sr;
    private Coroutine currentAnim;
    private bool isPlaying;

    public bool IsPlaying => isPlaying;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);

        if(sr != null)
        {
            var c = sr.color; c.a = 0f; sr.color = c;
        }

        if(startPositionObject != null && endPositionObject != null)
            currentAnim = StartCoroutine(PlayOneCycle(startPositionObject.position, endPositionObject.position));
        else
            Debug.LogWarning("Start atau End Position belum di-assign di HandHelp3.");
    }

    private IEnumerator PlayOneCycle(Vector3 from, Vector3 to)
    {
        yield return HandSwipe(from, to);
        currentAnim = null;
    }

    private IEnumerator HandSwipe(Vector3 from, Vector3 to)
    {
        if(sr == null) yield break;

        transform.position = from;
        var c = sr.color; c.a = 0f; sr.color = c;

        // fade in
        yield return Fade(0f, 1f, fadeDuration);

        // gerak ke target sambil fade out
        yield return MoveAndFadeOut(transform, from, to);

        // pastikan invisible di akhir
        c.a = 0f; sr.color = c;

        // kembalikan ke posisi awal
        transform.position = from;
    }


    private IEnumerator MoveAndFadeOut(Transform obj, Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float moved = 0f;
        float currentSpeed = baseSpeed;
        Vector3 dir = (to - from).normalized;
        obj.position = from;

        var c = sr.color;

        while(moved < distance)
        {
            float dt = Time.deltaTime;
            obj.position += dir * currentSpeed * dt;
            moved += currentSpeed * dt;
            currentSpeed += acceleration * dt;

            float progress = Mathf.Clamp01(moved / distance);

            if(progress >= fadeStartPercent)
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

        while(t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t/duration);
            sr.color = c;
            yield return null;
        }

        c.a = to; sr.color = c;
    }
}
