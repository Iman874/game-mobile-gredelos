using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class ClickSwipeAll : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorGameplay = 1; // Nomor gameplay

    [Header("Swipe Window Settings")]
    public Sprite[] steps;     // urutan sprite jendela
    public int totalSwipes = 3; // jumlah swipe total sampai terbuka penuh
    private SpriteRenderer sr;
    private int currentSwipe = 0;

    private Vector2 startPos;
    private bool isTouching = false;

    private Camera cam;
    private BoxCollider2D col;

    [Range(0f, 1f)]
    public float swipePercent = 0.3f; // minimal % panjang collider yang harus digeser

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = steps[0];
        cam = Camera.main;
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(touch.position.ReadValue());
            if (col.OverlapPoint(worldPos))
            {
                isTouching = true;
                startPos = touch.position.ReadValue();
            }
        }

        if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended && isTouching)
        {
            isTouching = false;

            Vector2 endPos = touch.position.ReadValue();
            float deltaX = endPos.x - startPos.x;

            // hitung panjang collider di screen space
            Vector3 left = cam.WorldToScreenPoint(col.bounds.min);
            Vector3 right = cam.WorldToScreenPoint(col.bounds.max);
            float colliderWidth = Mathf.Abs(right.x - left.x);

            float requiredSwipe = colliderWidth * swipePercent;

            if (Mathf.Abs(deltaX) >= requiredSwipe)
            {
                if (deltaX > 0) DoSwipeForward();
                else DoSwipeBackward();
            }
        }
    }

    private void DoSwipeForward()
    {
        if (currentSwipe < totalSwipes)
        {
            currentSwipe++;

            // hitung index sprite berdasarkan progress swipe
            int targetIndex = Mathf.RoundToInt(((float)currentSwipe / totalSwipes) * (steps.Length - 1));
            sr.sprite = steps[targetIndex];

            if (currentSwipe == totalSwipes)
            {
                Debug.Log("Jendela terbuka penuh (selesai).");
                // Panggil fungsi pada game controller
                var gameController = FindFirstObjectByType<ControllerPlayObjekClick>();
                if (gameController != null)
                {
                    gameController.OnSelesaiGameplay(nomorGameplay); // nomor gameplay 1
                }
                else
                {
                    Debug.LogWarning("GameController tidak ditemukan di scene.");
                }
            }
        }
    }

    private void DoSwipeBackward()
    {
        // sekarang bukan mundur, tapi sama kayak forward → tambah progress
        if (currentSwipe < totalSwipes)
        {
            currentSwipe++;
        }

        int targetIndex = Mathf.RoundToInt(((float)currentSwipe / totalSwipes) * (steps.Length - 1));
        sr.sprite = steps[targetIndex];

        if (currentSwipe == totalSwipes)
        {
            Debug.Log("Jendela terbuka penuh (selesai).");
            var gameController = FindFirstObjectByType<ControllerPlayObjekClick>();
            if (gameController != null)
            {
                gameController.OnSelesaiGameplay(nomorGameplay);
            }
            else
            {
                Debug.LogWarning("GameController tidak ditemukan di scene.");
            }
        }
    }
}
