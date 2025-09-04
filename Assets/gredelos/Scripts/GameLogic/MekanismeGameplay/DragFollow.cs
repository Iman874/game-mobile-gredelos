using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DragFollow : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorGameplay = 1;

    [Header("Swipe Settings")]
    public Sprite[] stepsLaki;
    public Sprite[] stepsPerempuan;
    public int totalSwipes = 3;
    [Range(0f,1f)] public float swipePercent = 0.3f;

    [Header("Target Swipe")]
    public GameObject targetObjectLaki;
    public GameObject targetObjectPerempuan;

    private Camera cam;
    private Collider2D col;
    private Collider2D targetCol;
    private SpriteRenderer srTarget;

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 startPosition;

    private Vector2 swipeStartPos;
    private bool swipeStarted = false;
    private int currentSwipe = 0;
    private Sprite[] currentSteps;

    void Start()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();
        startPosition = transform.position;

        // Tentukan target aktif & steps
        if (targetObjectLaki != null && targetObjectLaki.activeInHierarchy)
        {
            targetCol = targetObjectLaki.GetComponent<Collider2D>();
            srTarget = targetObjectLaki.GetComponent<SpriteRenderer>();
            currentSteps = stepsLaki;
        }
        else if (targetObjectPerempuan != null && targetObjectPerempuan.activeInHierarchy)
        {
            targetCol = targetObjectPerempuan.GetComponent<Collider2D>();
            srTarget = targetObjectPerempuan.GetComponent<SpriteRenderer>();
            currentSteps = stepsPerempuan;
        }

        if (srTarget != null && currentSteps.Length > 0)
            srTarget.sprite = currentSteps[0];
    }

    void Update()
    {
        HandleInput();

        // cek jika drag sudah masuk target
        if (isDragging && targetCol != null && col.IsTouching(targetCol) && !swipeStarted)
        {
            swipeStarted = true;
            swipeStartPos = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    private void HandleInput()
    {
        // Mouse
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartDrag(mousePos);
            else if (isDragging && Mouse.current.leftButton.isPressed)
                Drag(mousePos);
            else if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag(mousePos);
        }

        // Touch
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var touch = Touchscreen.current.touches[0];
            Vector2 touchPos = touch.position.ReadValue();

            switch (touch.phase.ReadValue())
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    StartDrag(touchPos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    if (isDragging) Drag(touchPos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (isDragging) EndDrag(touchPos);
                    break;
            }
        }
    }

    private void StartDrag(Vector2 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        if (col.OverlapPoint(worldPos))
        {
            isDragging = true;
            offset = transform.position - worldPos;
            swipeStarted = false;
        }
    }

    private void Drag(Vector2 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        transform.position = worldPos + offset;

        if (swipeStarted && targetCol != null)
        {
            Vector2 currentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            float deltaX = currentScreenPos.x - swipeStartPos.x;

            Vector3 left = cam.WorldToScreenPoint(targetCol.bounds.min);
            Vector3 right = cam.WorldToScreenPoint(targetCol.bounds.max);
            float targetWidth = Mathf.Abs(right.x - left.x);
            float requiredSwipe = targetWidth * swipePercent;

            if (Mathf.Abs(deltaX) >= requiredSwipe)
            {
                DoSwipe();
                swipeStarted = false;
            }
        }
    }

    private void EndDrag(Vector2 screenPos)
    {
        isDragging = false;
        transform.position = startPosition; // reset ke awal
    }

    private void DoSwipe()
    {
        if (currentSwipe < totalSwipes && srTarget != null && currentSteps != null)
        {
            currentSwipe++;
            int targetIndex = Mathf.RoundToInt(((float)currentSwipe / totalSwipes) * (currentSteps.Length - 1));
            srTarget.sprite = currentSteps[targetIndex];

            Debug.Log($"Swipe berhasil! Progress {currentSwipe}/{totalSwipes}");

            if (currentSwipe == totalSwipes)
            {
                Debug.Log("Swipe penuh, target sprite penuh!");
                var gameController = FindFirstObjectByType<ControllerPlayObjekLevel3>();
                if (gameController != null)
                    gameController.OnSelesaiGameplay(nomorGameplay);
            }
        }
    }
}
