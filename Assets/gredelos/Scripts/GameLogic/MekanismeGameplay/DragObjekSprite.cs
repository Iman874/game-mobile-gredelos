using UnityEngine;
using UnityEngine.InputSystem;

public class DragObjectSprite : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorLevel;
    public int nomorGameplay;

    [Header("Info Objek")]
    public string NameDragOpsional;
    public GameObject targetObject; // target game object yang punya collider
    private Vector3 startPosition; // posisi awal dragObject
    private bool isDragging = false;

    [Header("Swipe Setting")]
    public float swipeDistanceThreshold = 50f; // minimal jarak drag (pixel)
    public int minimalSwipe = 4; // jumlah swipe minimal sebelum trigger animasi

    private Vector2 dragStartScreenPos; // posisi awal untuk hitung jarak
    private int currentSwipeCount = 0; // counter swipe saat ini

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Mouse input (PC)
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

        // Touch input (Android/iOS)
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
        {
            isDragging = true;
            dragStartScreenPos = screenPosition;
            currentSwipeCount = 0; // reset counter setiap mulai drag baru
        }
    }

    private void Drag(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPos.z = 0;
        transform.position = worldPos;

        // cek jarak swipe
        float swipeDistance = Vector2.Distance(dragStartScreenPos, screenPosition);

        if (swipeDistance >= swipeDistanceThreshold)
        {
            dragStartScreenPos = screenPosition; // reset untuk swipe berikutnya
            currentSwipeCount++;

            Debug.Log($"Swipe ke-{currentSwipeCount}");

            if (currentSwipeCount >= minimalSwipe)
            {
                Debug.Log("Minimal swipe tercapai! Trigger animasi.");

                if (targetObject != null)
                {
                    Collider2D targetCollider = targetObject.GetComponent<Collider2D>();
                    if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
                    {
                        var spriteController = targetObject.GetComponent<SpriteController>();
                        if (spriteController != null)
                        {
                            int sisaSprite = -1;

                            if (NameDragOpsional == "self")
                            {
                                var selfSprite = GetComponent<SpriteController>();
                                if (selfSprite != null)
                                    sisaSprite = selfSprite.ChangeSprite();
                            }
                            else
                            {
                                sisaSprite = spriteController.ChangeSprite();
                            }

                            // kalau sprite sudah habis (sisa -1), balikin ke posisi awal
                            if (sisaSprite == 0)
                            {
                                Debug.Log("Sprite sudah habis, kembalikan objek ke posisi awal.");
                                transform.position = startPosition;
                                isDragging = false; // hentikan drag
                            }
                        }
                    }
                }

                // reset counter supaya butuh swipe ulang untuk trigger berikutnya
                currentSwipeCount = 0;
            }
        }
    }

    private void EndDrag()
    {
        isDragging = false;
        transform.position = startPosition;
        currentSwipeCount = 0; // reset saat lepas drag
    }
}
