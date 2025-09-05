using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DragMandiAir : MonoBehaviour
{
    [Header("Child")]
    public GameObject wadahKosongObj;  // versi kosong (misal ember kosong)
    public GameObject wadahIsiObj;     // versi isi air

    [Header("Target")]
    public GameObject targetBakMandi;   // ambil air
    public GameObject targetAreaMandi;  // tuangkan air

    private bool adaAir = false;
    private Vector3 posisiAwal;
    private bool isDragging = false;

    private void Start()
    {
        posisiAwal = transform.position;
        SetKosong();
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

        // ===== Cek ambil air langsung saat kena bak mandi =====
        if (targetBakMandi != null && !adaAir)
        {
            Collider2D targetCollider = targetBakMandi.GetComponent<Collider2D>();
            if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
            {
                adaAir = true;
                SetIsi();
                Debug.Log("Mengambil air dari bak mandi");
            }
        }
    }

    private void EndDrag()
    {
        isDragging = false;

        // cek tabrakan ke area mandi (tuangkan air)
        if (targetAreaMandi != null && adaAir)
        {
            Collider2D targetCollider = targetAreaMandi.GetComponent<Collider2D>();
            if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
            {
                adaAir = false;
                SetKosong();
                Debug.Log("Menuangkan air ke area mandi!");
                KembaliKeAwal();

                // Panggil SpriteControllerMandi untuk efek basah
                var spriteController = FindObjectOfType<SpriteControllerMandi>();
                if (spriteController != null)
                {
                    spriteController.ChangeSpriteMandi();
                }
                else
                {
                    Debug.LogWarning("SpriteControllerMandi tidak ditemukan di scene.");
                }

                return;
            }
        }

        // kalau gagal, kembali ke posisi awal
        if (!adaAir) KembaliKeAwal();
    }

    private void SetKosong()
    {
        if (wadahKosongObj) wadahKosongObj.SetActive(true);
        if (wadahIsiObj) wadahIsiObj.SetActive(false);
    }

    private void SetIsi()
    {
        if (wadahKosongObj) wadahKosongObj.SetActive(false);
        if (wadahIsiObj) wadahIsiObj.SetActive(true);
    }

    private void KembaliKeAwal()
    {
        transform.position = posisiAwal;
    }
}
