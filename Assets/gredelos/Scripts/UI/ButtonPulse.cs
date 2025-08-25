using UnityEngine;

public class ButtonPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseScale = 1.1f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1);
        transform.localScale = originalScale * scale;
    }
}
