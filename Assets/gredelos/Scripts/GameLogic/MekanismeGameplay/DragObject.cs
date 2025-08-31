using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorLevel;
    public int nomorGameplay;
    public GameObject ControllerPlayObject;

    [Header("Info Objek")]
    public GameObject targetObject; // target game object yang punya collider
    private Vector3 startPosition; // posisi awal dragObject
    private bool isDragging = false;

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

        // cek apakah collider dari targetObject disentuh
        if (targetObject != null)
        {
            Collider2D targetCollider = targetObject.GetComponent<Collider2D>();
            if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
            {
                Debug.Log("Berhasil!");
                // Ambil fungsi dari ControllerPlayObject
                if (ControllerPlayObject != null && nomorLevel == 3)
                {
                    ControllerPlayObject.GetComponent<ControllerPlayObjekLevel3>().OnProgress(nomorGameplay, nomorLevel);
                    // Nonaktifkan objek
                    gameObject.SetActive(false);
                }
            }
        }

        // kalau gagal, balik ke posisi awal
        transform.position = startPosition;
    }
}
