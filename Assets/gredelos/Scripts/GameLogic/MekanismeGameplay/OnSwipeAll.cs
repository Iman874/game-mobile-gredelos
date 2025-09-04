using UnityEngine;
using UnityEngine.InputSystem;

// bikin alias supaya ga bentrok dengan UnityEngine.TouchPhase lama
using InputTouchPhase = UnityEngine.InputSystem.TouchPhase;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class OnSwipeAll : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorGameplay = 1;

    [Header("Swipe Window Settings")]
    public Sprite[] steps;         // urutan sprite jendela
    public int totalSwipes = 3;    // jumlah swipe total
    [Range(0f, 1f)]
    public float swipePercent = 0.3f; // minimal % panjang collider yang harus digeser

    private SpriteRenderer sr;
    private int currentSwipe = 0;

    private Camera cam;
    private BoxCollider2D col;

    private Vector2 startPos;
    private bool isTouching = false;
    private bool startFromLeft = false;
    private bool startFromRight = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (steps != null && steps.Length > 0)
            sr.sprite = steps[0];

        cam = Camera.main;
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Touchscreen.current == null) return;
        var touch = Touchscreen.current.primaryTouch;

        // ----------------- TOUCH BEGAN -----------------
        if (touch.phase.ReadValue() == InputTouchPhase.Began)
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(touch.position.ReadValue());
            if (col.OverlapPoint(worldPos))
            {
                isTouching = true;
                startPos = touch.position.ReadValue();

                // tentukan posisi awal (kiri atau kanan collider)
                Vector3 left = cam.WorldToScreenPoint(col.bounds.min);
                Vector3 right = cam.WorldToScreenPoint(col.bounds.max);

                float startX = startPos.x;

                float boundaryOffset = (right.x - left.x) * 0.2f; // 20% area ujung dianggap "paling kiri/kanan"
                startFromLeft = startX <= left.x + boundaryOffset;
                startFromRight = startX >= right.x - boundaryOffset;
            }
        }

        // ----------------- TOUCH ENDED -----------------
        if (touch.phase.ReadValue() == InputTouchPhase.Ended && isTouching)
        {
            isTouching = false;

            Vector2 endPos = touch.position.ReadValue();
            float deltaX = endPos.x - startPos.x;

            Vector3 left = cam.WorldToScreenPoint(col.bounds.min);
            Vector3 right = cam.WorldToScreenPoint(col.bounds.max);
            float colliderWidth = Mathf.Abs(right.x - left.x);

            float requiredSwipe = colliderWidth * swipePercent;

            // Syarat swipe valid:
            bool validSwipe = false;
            float midX = (left.x + right.x) / 2f;

            if (startFromLeft && deltaX > 0 && endPos.x >= midX && Mathf.Abs(deltaX) >= requiredSwipe)
            {
                validSwipe = true;
            }
            else if (startFromRight && deltaX < 0 && endPos.x <= midX && Mathf.Abs(deltaX) >= requiredSwipe)
            {
                validSwipe = true;
            }

            if (validSwipe)
            {
                DoSwipe();
            }
        }
    }

    private void DoSwipe()
    {
        if (currentSwipe < totalSwipes)
        {
            currentSwipe++;
            int targetIndex = Mathf.RoundToInt(((float)currentSwipe / totalSwipes) * (steps.Length - 1));
            sr.sprite = steps[targetIndex];

            if (currentSwipe == totalSwipes)
            {
                Debug.Log("Swipe selesai → sprite penuh terbuka.");

                if (nomorGameplay == 7)
                {
                    // Tambahkan logika khusus untuk nomorGameplay 7
                    var gameController = FindFirstObjectByType<ControllerPlayObjekLevel3>();
                    if (gameController != null)
                        gameController.OnSelesaiGameplay(nomorGameplay);

                }
               
            }
        }
    }
}
