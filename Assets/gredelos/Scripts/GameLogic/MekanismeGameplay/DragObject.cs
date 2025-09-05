using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorLevel;
    public int nomorGameplay;

    [Header("Info Objek")]
    public string NameDragOpsional;
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

        if (targetObject != null)
        {
            Collider2D targetCollider = targetObject.GetComponent<Collider2D>();
            if (targetCollider != null && GetComponent<Collider2D>().IsTouching(targetCollider))
            {
                Debug.Log("Berhasil!");

                // Cari controller di sini, baru panggil OnProgress
                GameObject controller = null;
                switch (nomorLevel)
                {
                    case 3:
                        controller = GameObject.FindAnyObjectByType<ControllerPlayObjekLevel3>()?.gameObject;
                        controller?.GetComponent<ControllerPlayObjekLevel3>()?.OnProgress(nomorGameplay, nomorLevel);
                        this.gameObject.SetActive(false); // nonaktifkan objek drag setelah berhasil
                        break;
                    case 2:
                        controller = GameObject.FindAnyObjectByType<ControllerPlayObjekLevel2>()?.gameObject;
                        controller?.GetComponent<ControllerPlayObjekLevel2>()?.OnSelesaiProgress(nomorGameplay, nomorLevel);
                        break;
                    case 4:
                        controller = GameObject.FindAnyObjectByType<ControllerPlayObjekLevel4>()?.gameObject;
                        controller?.GetComponent<ControllerPlayObjekLevel4>()?.OnProgress(nomorGameplay, nomorLevel, NameDragOpsional);
                        break;
                    case 5:
                        controller = GameObject.FindAnyObjectByType<ControllerPlayObjekLevel5>()?.gameObject;
                        controller?.GetComponent<ControllerPlayObjekLevel5>()?.OnSelesaiProgress(nomorGameplay, nomorLevel);
                        break;
                    // tambah level lain jika perlu
                }
            }
        }

        // gagal, kembali ke posisi awal
        transform.position = startPosition;
    }

}
