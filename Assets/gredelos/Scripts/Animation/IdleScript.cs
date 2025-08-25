using UnityEngine;

public class IdleScript : MonoBehaviour
{
    public Animator animator;
    public string idleTriggerName = "IdleAction";
    public float extraIdleDelay = 3f; // jeda setelah animasi idle selesai

    private float idleAnimLength;     // durasi animasi idle
    private float timer;
    private bool ready = false;

    void Start()
    {
        // Ambil panjang animasi idle dari Animator (state awal)
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
        {
            idleAnimLength = clips[0].clip.length;
        }
        else
        {
            Debug.LogWarning("Tidak ditemukan animasi aktif di Animator Layer 0.");
            idleAnimLength = 0f;
        }

        timer = 0f;
        ready = true;
    }

    void Update()
    {
        if (!ready) return;

        timer += Time.deltaTime;

        if (timer >= idleAnimLength + extraIdleDelay)
        {
            timer = 0f;
            Debug.Log("Triggering idle animation at: " + Time.time);
            animator.SetTrigger(idleTriggerName);
        }
    }
}
