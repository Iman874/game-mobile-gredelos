using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DragSendok : MonoBehaviour
{
    [Header("Child Sendok")]
    public GameObject sendokKosongObj;  // drag di inspector
    public GameObject sendokIsiObj;     // drag di inspector

    [Header("Target")]
    public GameObject targetPiring;     // drag di inspector
    public GameObject targetMulut;      // drag di inspector

    private bool adaMakanan = false;
    private Vector3 posisiAwal;
    private bool isDragging = false;

    private void Start()
    {
        posisiAwal = transform.position;
        SetSendokKosong();
    }

    private void Update()
    {
        // ===== Mouse Input =====
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                CheckStartDrag(mousePos);
            else if (isDragging && Mouse.current.leftButton.isPressed)
                Drag(mousePos);
            else if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag();
        }

        // ===== Touch Input =====
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var touch = Touchscreen.current.touches[0];
            Vector2 touchPos = touch.position.ReadValue();

            switch (touch.phase.ReadValue())
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    CheckStartDrag(touchPos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    if (isDragging) Drag(touchPos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (isDragging) EndDrag();
                    break;
            }
        }
    }

    private void CheckStartDrag(Vector2 screenPosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.transform == transform)
            isDragging = true;
    }

    private void Drag(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPos.z = 0;
        transform.position = worldPos;
    }

    private void EndDrag()
    {
        isDragging = false;

        // cek tabrakan ke piring
        if (targetPiring != null && !adaMakanan)
        {
            Collider2D targetCollider = targetPiring.GetComponent<Collider2D>();
            if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
            {
                adaMakanan = true;
                SetSendokIsi();
                Debug.Log("Sendok ambil makanan");
                KembaliKeAwal();
                return;
            }
        }

        // cek tabrakan ke mulut
        if (targetMulut != null && adaMakanan)
        {
            Collider2D targetCollider = targetMulut.GetComponent<Collider2D>();
            if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
            {
                adaMakanan = false;
                SetSendokKosong();
                Debug.Log("Berhasil memberi makan!");
                // Cari Script component karakter makan yang sedang aktif
                KarakterMakan karakter = FindObjectOfType<KarakterMakan>();
                if (karakter != null && karakter.enabled)
                {
                    karakter.Makan();
                    Debug.Log("Berhasil Putar Animasi Makan");
                    // Putar suara makan
                    ManagerAudio.instance.PlayMakanSound();
                }
                else
                {
                    Debug.Log("Karakter makan tidak aktif atau tidak ada discane");
                }
                KembaliKeAwal();
                return;
            }
        }

        // kalau gagal, kembali ke posisi awal
        if (!adaMakanan) KembaliKeAwal();
    }

    private void SetSendokKosong()
    {
        if (sendokKosongObj) sendokKosongObj.SetActive(true);
        if (sendokIsiObj) sendokIsiObj.SetActive(false);
    }

    private void SetSendokIsi()
    {
        if (sendokKosongObj) sendokKosongObj.SetActive(true);
        if (sendokIsiObj) sendokIsiObj.SetActive(true);
    }

    private void KembaliKeAwal()
    {
        transform.position = posisiAwal;
    }
}
