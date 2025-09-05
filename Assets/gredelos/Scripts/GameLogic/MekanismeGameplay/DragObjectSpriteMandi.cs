using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DragObjectSpriteMandi : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorLevel;
    public int nomorGameplay;

    [Header("Target Collider")]
    public GameObject targetArea; // cuma area collider, bukan yang punya SpriteController

    [Header("Daftar Objek yang Diganti Sprite-nya")]
    public List<GameObject> spriteObjects = new List<GameObject>();

    private Vector3 startPosition;
    private bool isDragging = false;

    [Header("Swipe Setting")]
    public float swipeDistanceThreshold = 50f; 
    public int minimalSwipe = 20; 

    private Vector2 dragStartScreenPos;
    private int currentSwipeCount = 0;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Mouse input
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

        // Touch input
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
            currentSwipeCount = 0;
        }
    }

    private void Drag(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPos.z = 0;
        transform.position = worldPos;

        float swipeDistance = Vector2.Distance(dragStartScreenPos, screenPosition);

        if (swipeDistance >= swipeDistanceThreshold)
        {
            dragStartScreenPos = screenPosition;
            currentSwipeCount++;

            Debug.Log($"Swipe ke-{currentSwipeCount}");

            if (currentSwipeCount >= minimalSwipe)
            {
                Debug.Log("Minimal swipe tercapai! Cek target collider.");

                if (targetArea != null)
                {
                    Collider2D targetCollider = targetArea.GetComponent<Collider2D>();
                    if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
                    {
                        // ubah semua sprite dari list yang aktif
                        foreach (var obj in spriteObjects)
                        {
                            if (obj == null || !obj.activeInHierarchy) continue;

                            var spriteController = obj.GetComponent<SpriteControllerMandi>();
                            if (spriteController != null)
                            {
                                int sisaSprite = spriteController.ChangeSpriteMandi();
                                Debug.Log($"Objek {obj.name} → sisa sprite: {sisaSprite}");

                                if (sisaSprite == 0)
                                {
                                    // setelah trigger == 0, balikin ke posisi awal
                                    transform.position = startPosition;
                                    isDragging = false;
                                }
                            }
                        }

                       
                    }
                }

                currentSwipeCount = 0;
            }
        }
    }

    private void EndDrag()
    {
        isDragging = false;
        transform.position = startPosition;
        currentSwipeCount = 0;
    }
}
