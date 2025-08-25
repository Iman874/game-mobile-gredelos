using UnityEngine;
using System.Collections;

public class AnimatorScale : MonoBehaviour
{
    [Header("Animasi Scale")]
    public float animDuration = 0.3f;
    public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine currentAnim;
    private Vector3 defaultScale; // <- simpan scale asli

    void Awake()
    {
        // Ambil scale default dari inspector
        defaultScale = transform.localScale;
    }

    // Fungsi panggil show animasi
    public void PlayShow()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(ScaleRoutine(Vector3.zero, defaultScale));
    }

    // Fungsi panggil hide animasi
    public void PlayHide()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(ScaleRoutine(transform.localScale, Vector3.zero, () =>
        {
            gameObject.SetActive(false); // setelah mengecil, nonaktifkan object
        }));
    }

    // Coroutine scale animasi
    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, System.Action onComplete = null)
    {
        float time = 0f;
        transform.localScale = from;

        while (time < animDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / animDuration;
            float scale = animCurve.Evaluate(t);
            transform.localScale = Vector3.Lerp(from, to, scale);

            yield return null;
        }

        transform.localScale = to;
        onComplete?.Invoke();
    }
}
